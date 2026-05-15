using System.Collections;
using System.Reflection;
using FluentAssertions;

namespace LADCI.AI.Invoicing.UnitTests.Domain;

public sealed class InvoiceTests
{
    [Fact]
    public void Create_Should_Assign_A_Unique_Identity()
    {
        var invoice = InvoiceTestBuilder.CreateValidInvoice();

        var invoiceId = InvoiceReflection.Property(invoice, "Id", "InvoiceId");

        invoiceId.Should().NotBeNull("an invoice must have a unique identity");
    }

    [Fact]
    public void Create_Should_Raise_InvoiceCreated_Domain_Event()
    {
        var invoice = InvoiceTestBuilder.CreateValidInvoice();

        InvoiceReflection.DomainEvents(invoice)
            .Select(static e => e.GetType().Name)
            .Should()
            .Contain("InvoiceCreated");
    }

    [Fact]
    public void Create_Should_Fail_When_InvoiceId_Is_Missing()
    {
        Action act = () => InvoiceTestBuilder.CreateInvoice(invoiceId: null, autoGenerateInvoiceId: false);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Create_Should_Fail_When_There_Are_No_Line_Items()
    {
        Action act = () => InvoiceTestBuilder.CreateInvoice(lineItems: []);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Create_Should_Fail_When_Total_Does_Not_Match_Line_Item_Sum()
    {
        var lineItems = new[]
        {
            InvoiceTestBuilder.CreateLineItem(40m),
            InvoiceTestBuilder.CreateLineItem(60m)
        };

        Action act = () => InvoiceTestBuilder.CreateInvoice(
            lineItems: lineItems,
            totalAmount: InvoiceTestBuilder.CreateMoney(99m));

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Create_Should_Succeed_When_Total_Equals_The_Sum_Of_Line_Items()
    {
        var lineItems = new[]
        {
            InvoiceTestBuilder.CreateLineItem(40m),
            InvoiceTestBuilder.CreateLineItem(60m)
        };

        var invoice = InvoiceTestBuilder.CreateInvoice(
            lineItems: lineItems,
            totalAmount: InvoiceTestBuilder.CreateMoney(100m));

        var total = InvoiceReflection.Property(invoice, "TotalAmount", "Amount", "Total");
        var amount = InvoiceReflection.DecimalValue(total);

        amount.Should().Be(100m);
    }

    [Fact]
    public void Create_Should_Support_A_Single_Line_Item_When_Total_Is_Exact()
    {
        var invoice = InvoiceTestBuilder.CreateInvoice(
            lineItems: [InvoiceTestBuilder.CreateLineItem(100m)],
            totalAmount: InvoiceTestBuilder.CreateMoney(100m));

        var total = InvoiceReflection.Property(invoice, "TotalAmount", "Amount", "Total");

        InvoiceReflection.DecimalValue(total).Should().Be(100m);
    }

    [Fact]
    public void MarkAsPaid_Should_Set_Status_To_Paid()
    {
        var invoice = InvoiceTestBuilder.CreateValidInvoice();

        InvoiceReflection.MarkAsPaid(invoice);

        var status = InvoiceReflection.Property(invoice, "Status");

        status.Should().NotBeNull();
        status!.ToString().Should().Be("Paid");
    }

    [Fact]
    public void MarkAsPaid_Should_Be_An_Instance_Operation_On_An_Existing_Invoice()
    {
        var invoiceType = InvoiceReflection.RequiredType("Invoice");

        invoiceType.GetMethod("MarkAsPaid", BindingFlags.Instance | BindingFlags.Public)
            .Should()
            .NotBeNull("marking an invoice as paid should be behavior on an existing aggregate instance");
    }

    [Fact]
    public void MarkAsPaid_Should_Raise_InvoiceMarkedAsPaid_Domain_Event()
    {
        var invoice = InvoiceTestBuilder.CreateValidInvoice();

        InvoiceReflection.MarkAsPaid(invoice);

        InvoiceReflection.DomainEvents(invoice)
            .Select(static e => e.GetType().Name)
            .Should()
            .Contain("InvoiceMarkedAsPaid");
    }

    [Fact]
    public void MarkAsPaid_Should_Fail_When_Invoice_Is_Already_Paid()
    {
        var invoice = InvoiceTestBuilder.CreateValidInvoice();
        InvoiceReflection.MarkAsPaid(invoice);

        Action act = () => InvoiceReflection.MarkAsPaid(invoice);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void MarkAsPaid_Should_Only_Raise_InvoiceMarkedAsPaid_Once()
    {
        var invoice = InvoiceTestBuilder.CreateValidInvoice();
        InvoiceReflection.MarkAsPaid(invoice);

        InvoiceReflection.DomainEvents(invoice)
            .Count(static e => e.GetType().Name == "InvoiceMarkedAsPaid")
            .Should()
            .Be(1);
    }
}

internal static class InvoiceTestBuilder
{
    public static object CreateValidInvoice()
    {
        var lineItems = new[]
        {
            CreateLineItem(100m)
        };

        return CreateInvoice(
            invoiceId: CreateInvoiceId(),
            customerId: CreateCustomerId(),
            issueDate: DateOnly.FromDateTime(DateTime.UtcNow),
            dueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)),
            lineItems: lineItems,
            totalAmount: CreateMoney(100m));
    }

    public static object CreateInvoice(
        object? invoiceId = null,
        object? customerId = null,
        DateOnly? issueDate = null,
        DateOnly? dueDate = null,
        IReadOnlyCollection<object>? lineItems = null,
        object? totalAmount = null,
        bool autoGenerateInvoiceId = true)
    {
        var invoiceType = InvoiceReflection.RequiredType("Invoice");
        var actualIssueDate = issueDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var actualDueDate = dueDate ?? actualIssueDate.AddDays(14);
        var actualLineItems = lineItems ?? [CreateLineItem(100m)];
        var actualTotal = totalAmount ?? CreateMoney(actualLineItems.Sum(CreateLineItemAmount));
        var resolvedInvoiceId = autoGenerateInvoiceId ? invoiceId ?? CreateInvoiceId() : invoiceId;

        var parameterBag = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["invoiceid"] = resolvedInvoiceId,
            ["id"] = resolvedInvoiceId,
            ["customerid"] = customerId ?? CreateCustomerId(),
            ["issuedate"] = actualIssueDate,
            ["date"] = actualIssueDate,
            ["duedate"] = CreateDueDate(actualDueDate),
            ["lineitems"] = actualLineItems.ToArray(),
            ["items"] = actualLineItems.ToArray(),
            ["totalamount"] = actualTotal,
            ["total"] = actualTotal
        };

        return InvoiceReflection.CreateAggregateInstance(invoiceType, parameterBag);
    }

    public static object CreateInvoiceId()
        => InvoiceReflection.CreateValueObject("InvoiceId", Guid.NewGuid(), Guid.NewGuid().ToString("N"));

    public static object CreateCustomerId()
        => InvoiceReflection.CreateValueObject("CustomerId", Guid.NewGuid(), Guid.NewGuid().ToString("N"));

    public static object CreateDueDate(DateOnly value)
        => InvoiceReflection.CreateOptionalValueObject("DueDate", value) ?? value;

    public static object CreateMoney(decimal amount)
        => InvoiceReflection.CreateMoney(amount, "EUR");

    public static object CreateLineItem(decimal subtotal)
    {
        var lineItemType = InvoiceReflection.RequiredType("LineItem");
        var factories = lineItemType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name is "Create" or "New" or "Of" or "From")
            .ToArray();

        foreach (var factory in factories)
        {
            if (InvoiceReflection.TryBuildArguments(factory.GetParameters(), out var args, subtotal))
            {
                return factory.Invoke(null, args)
                    ?? throw new InvalidOperationException("LineItem factory returned null.");
            }
        }

        foreach (var ctor in lineItemType.GetConstructors())
        {
            if (InvoiceReflection.TryBuildArguments(ctor.GetParameters(), out var args, subtotal))
            {
                return ctor.Invoke(args);
            }
        }

        throw new InvalidOperationException(
            "Could not create LineItem. Add a supported constructor or static factory so tests can instantiate the value object.");
    }

    public static decimal CreateLineItemAmount(object lineItem)
    {
        var candidate = InvoiceReflection.Property(lineItem, "Subtotal", "Amount", "Total", "Value");
        return InvoiceReflection.DecimalValue(candidate);
    }
}

internal static class InvoiceReflection
{
    private static readonly Assembly DomainAssembly = Assembly.Load("LADCI.AI.Invoicing.Domain");

    public static Type RequiredType(string typeName)
        => DomainAssembly.GetTypes().SingleOrDefault(t => t.Name == typeName)
           ?? throw new InvalidOperationException(
               $"Type '{typeName}' was not found in the Domain assembly. Implement the domain model before running this specification.");

    public static object CreateValueObject(string typeName, Guid guidValue, string stringValue)
    {
        var type = RequiredType(typeName);

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (method.Name is not ("Create" or "New" or "Of" or "From"))
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Guid))
            {
                return method.Invoke(null, [guidValue])!;
            }

            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
            {
                return method.Invoke(null, [stringValue])!;
            }
        }

        foreach (var ctor in type.GetConstructors())
        {
            var parameters = ctor.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Guid))
            {
                return ctor.Invoke([guidValue]);
            }

            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
            {
                return ctor.Invoke([stringValue]);
            }
        }

        throw new InvalidOperationException($"Could not create value object '{typeName}'.");
    }

    public static object? CreateOptionalValueObject(string typeName, DateOnly value)
    {
        var type = DomainAssembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
        if (type is null)
        {
            return null;
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (method.Name is not ("Create" or "New" or "Of" or "From"))
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(DateOnly))
            {
                return method.Invoke(null, [value]);
            }
        }

        foreach (var ctor in type.GetConstructors())
        {
            var parameters = ctor.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(DateOnly))
            {
                return ctor.Invoke([value]);
            }
        }

        return null;
    }

    public static object CreateMoney(decimal amount, string currency)
    {
        var type = RequiredType("Money");

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (method.Name is not ("Create" or "New" or "Of" or "From"))
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length == 2 && parameters[0].ParameterType == typeof(decimal) && parameters[1].ParameterType == typeof(string))
            {
                return method.Invoke(null, [amount, currency])!;
            }

            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(decimal))
            {
                return method.Invoke(null, [amount])!;
            }
        }

        foreach (var ctor in type.GetConstructors())
        {
            var parameters = ctor.GetParameters();
            if (parameters.Length == 2 && parameters[0].ParameterType == typeof(decimal) && parameters[1].ParameterType == typeof(string))
            {
                return ctor.Invoke([amount, currency]);
            }

            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(decimal))
            {
                return ctor.Invoke([amount]);
            }
        }

        throw new InvalidOperationException("Could not create Money value object.");
    }

    public static object CreateAggregateInstance(Type aggregateType, IReadOnlyDictionary<string, object?> parameterBag)
    {
        var factories = aggregateType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name is "Create" or "New" or "Issue")
            .ToArray();

        foreach (var factory in factories)
        {
            if (TryResolveArguments(factory.GetParameters(), parameterBag, out var args))
            {
                return factory.Invoke(null, args)
                    ?? throw new InvalidOperationException($"{aggregateType.Name} factory returned null.");
            }
        }

        foreach (var ctor in aggregateType.GetConstructors())
        {
            if (TryResolveArguments(ctor.GetParameters(), parameterBag, out var args))
            {
                return ctor.Invoke(args);
            }
        }

        throw new InvalidOperationException(
            $"Could not construct aggregate '{aggregateType.Name}'. Add a public constructor or static factory compatible with the domain model.");
    }

    public static IReadOnlyCollection<object> DomainEvents(object aggregate)
    {
        var value = Property(aggregate, "DomainEvents", "Events", "UncommittedEvents");
        value.Should().NotBeNull("aggregates should expose domain events for verification");

        return value switch
        {
            IEnumerable<object> typed => typed.ToArray(),
            IEnumerable untyped => untyped.Cast<object>().ToArray(),
            _ => throw new InvalidOperationException("Domain events property must be enumerable.")
        };
    }

    public static object? Property(object instance, params string[] names)
    {
        var type = instance.GetType();

        foreach (var name in names)
        {
            var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (property is not null)
            {
                return property.GetValue(instance);
            }
        }

        throw new InvalidOperationException(
            $"None of the properties [{string.Join(", ", names)}] were found on type '{type.Name}'.");
    }

    public static void MarkAsPaid(object invoice)
    {
        var method = invoice.GetType().GetMethod("MarkAsPaid", BindingFlags.Instance | BindingFlags.Public);
        method.Should().NotBeNull("Invoice aggregate should expose a MarkAsPaid use case");

        method!.Invoke(invoice, []);
    }

    public static decimal DecimalValue(object? value)
    {
        value.Should().NotBeNull();

        if (value is decimal decimalValue)
        {
            return decimalValue;
        }

        var type = value!.GetType();
        var property = type.GetProperty("Amount") ?? type.GetProperty("Value");
        if (property is not null)
        {
            var innerValue = property.GetValue(value);
            if (innerValue is decimal innerDecimal)
            {
                return innerDecimal;
            }
        }

        throw new InvalidOperationException($"Could not extract decimal value from '{type.Name}'.");
    }

    public static bool TryBuildArguments(ParameterInfo[] parameters, out object?[] arguments, decimal subtotal)
    {
        var values = new List<object?>(parameters.Length);

        foreach (var parameter in parameters)
        {
            var type = parameter.ParameterType;
            var name = parameter.Name ?? string.Empty;

            if (type == typeof(string))
            {
                values.Add(name.Contains("currency", StringComparison.OrdinalIgnoreCase) ? "EUR" : "Consulting");
                continue;
            }

            if (type == typeof(int))
            {
                values.Add(1);
                continue;
            }

            if (type == typeof(decimal))
            {
                values.Add(subtotal);
                continue;
            }

            if (type == typeof(Guid))
            {
                values.Add(Guid.NewGuid());
                continue;
            }

            if (type == typeof(DateOnly))
            {
                values.Add(DateOnly.FromDateTime(DateTime.UtcNow));
                continue;
            }

            if (type.Name == "Money")
            {
                values.Add(CreateMoney(subtotal, "EUR"));
                continue;
            }

            if (type.Name == "InvoiceId")
            {
                values.Add(CreateValueObject("InvoiceId", Guid.NewGuid(), Guid.NewGuid().ToString("N")));
                continue;
            }

            if (type.Name == "CustomerId")
            {
                values.Add(CreateValueObject("CustomerId", Guid.NewGuid(), Guid.NewGuid().ToString("N")));
                continue;
            }

            if (type.Name == "DueDate")
            {
                values.Add(CreateOptionalValueObject("DueDate", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)))
                    ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)));
                continue;
            }

            if (parameter.HasDefaultValue)
            {
                values.Add(parameter.DefaultValue);
                continue;
            }

            arguments = [];
            return false;
        }

        arguments = values.ToArray();
        return true;
    }

    private static bool TryResolveArguments(
        ParameterInfo[] parameters,
        IReadOnlyDictionary<string, object?> parameterBag,
        out object?[] arguments)
    {
        var values = new List<object?>(parameters.Length);

        foreach (var parameter in parameters)
        {
            if (TryResolveSingleArgument(parameter, parameterBag, out var value))
            {
                values.Add(value);
                continue;
            }

            arguments = [];
            return false;
        }

        arguments = values.ToArray();
        return true;
    }

    private static bool TryResolveSingleArgument(
        ParameterInfo parameter,
        IReadOnlyDictionary<string, object?> parameterBag,
        out object? value)
    {
        var name = parameter.Name ?? string.Empty;
        var type = parameter.ParameterType;

        if (parameterBag.TryGetValue(name, out value) && IsAssignable(value, type))
        {
            return true;
        }

        if (TryResolveBySemanticName(name, type, parameterBag, out value))
        {
            return true;
        }

        if (TryResolveCollection(type, parameterBag, out value))
        {
            return true;
        }

        if (parameter.HasDefaultValue)
        {
            value = parameter.DefaultValue;
            return true;
        }

        value = null;
        return false;
    }

    private static bool TryResolveBySemanticName(
        string parameterName,
        Type parameterType,
        IReadOnlyDictionary<string, object?> parameterBag,
        out object? value)
    {
        var normalizedName = parameterName.Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase);
        var keys = parameterBag.Keys.ToArray();

        foreach (var key in keys)
        {
            if (!normalizedName.Contains(key, StringComparison.OrdinalIgnoreCase)
                && !key.Contains(normalizedName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var candidate = parameterBag[key];
            if (IsAssignable(candidate, parameterType))
            {
                value = candidate;
                return true;
            }
        }

        value = null;
        return false;
    }

    private static bool TryResolveCollection(
        Type parameterType,
        IReadOnlyDictionary<string, object?> parameterBag,
        out object? value)
    {
        value = null;

        if (!typeof(IEnumerable).IsAssignableFrom(parameterType) || parameterType == typeof(string))
        {
            return false;
        }

        foreach (var key in new[] { "lineitems", "items" })
        {
            if (!parameterBag.TryGetValue(key, out var candidate) || candidate is null)
            {
                continue;
            }

            if (parameterType.IsInstanceOfType(candidate))
            {
                value = candidate;
                return true;
            }

            if (candidate is Array array && parameterType.IsArray)
            {
                value = array;
                return true;
            }

            if (candidate is Array sourceArray && parameterType.IsGenericType)
            {
                var itemType = parameterType.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(itemType);
                var list = (IList)Activator.CreateInstance(listType)!;

                var allAssignable = true;
                foreach (var item in sourceArray)
                {
                    if (item is null || !itemType.IsInstanceOfType(item))
                    {
                        allAssignable = false;
                        break;
                    }

                    list.Add(item);
                }

                if (allAssignable)
                {
                    value = list;
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsAssignable(object? candidate, Type targetType)
    {
        if (candidate is null)
        {
            return !targetType.IsValueType || Nullable.GetUnderlyingType(targetType) is not null;
        }

        return targetType.IsInstanceOfType(candidate);
    }
}
