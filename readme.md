## Overview

Charites.Bindings is a class library that contains properties for data bindings. They are mainly two properties as follows.

### ObservableProperty

The ObservableProperty provides notifications when a value is changed and validations of a value.

The ObservableProperty implements the INotifyPropertyChanged interface and raises the PropertyChanged event when the Value property is changed. This property can be used for data bindings. For example in WinUI, a model code is as follows:

``` csharp
public class User
{
    public ObservableProperty<string> Name { get; } = string.Empty.ToObservableProperty();
    public ObservableProperty<string> Address { get; } = string.Empty.ToObservableProperty();
}
```

When this model is set to a data context, XAML code is as follows:

``` xml
<TextBlock Text="{Binding Name.Value}" />
<TextBlock Text="{Binding Address.Value}" />
```

The ObservableProperty can also bind another ObservableProperty as follows:

``` csharp
public class Content
{
    public ObservableProperty<bool> IsExecuting { get; } = new ObservableProperty<bool>(false);
    public ObservableProperty<bool> CanExecute { get; } = new ObservableProperty<bool>(false);

    public Content()
    {
        CanExecute.Bind(IsExecuting, value => !value);
    }
}
```

Multiple ObservableProperty can be bound as follows:

``` csharp
public class Content
{
    public ObservableProperty<int> Red { get; } = new ObservableProperty<int>(0);
    public ObservableProperty<int> Green { get; } = new ObservableProperty<int>(0);
    public ObservableProperty<int> Blue { get; } = new ObservableProperty<int>(0);
    public ObservableProperty<Color> Color { get; } = new ObservableProperty<Color>(Colors.Black);

    public Content()
    {
        Color.Bind(
            args => Color.FromArgb(args.GetValueAt<int>(0), args.GetValueAt<int>(1), args.GetValueAt<int>(2)),
            Red, Green, Blue
        );
    }
}
```

The ObservableProperty can bind another ObservableProperty in the two-way direction as follows:

``` csharp
public class Content
{
    public ObservableProperty<int> Age { get; } = new ObservableProperty<int>(0);
    public ObservableProperty<string> InputAge { get; } = new ObservableProperty<string>(string.Empty);

    public Content()
    {
        Age.BindTwoWay(InputAge, int.Parse, value => value.ToString())
    }
}
```

In order that the ObservableProperty implements IDataErrorInfo and INotifyDataErrorInfo interfaces, the value of the ObservableProperty can also be validated using the ValidationAttribute as follows:

``` csharp
public class User
{
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "Please enter {0}.")]
    [StringLength(25, ErrorMessage = "Please enter {0} within {1} characters.")]
    public ObservableProperty<string> FirstName { get; } = new ObservableProperty<string>(string.Empty);

    public User()
    {
        FirstName.EnableValidation(() => FirstName);
    }
}
```

The ObservableProperty can delay a value change with the specified time span as follows:

``` csharp
public class Content
{
    public ObservableProperty<string> SearchCriteria { get; } = new ObservableProperty<string>(string.Empty);

    public Content()
    {
        SearchCriteria.EnableDelayValueChange(TimeSpan.FromMilliseconds(500));
    }
}
```

The value change is executed with the specified SynchronizationContext. The defalut value is the SynchronizationContext.Current.

### BoundProperty

The BoundProperty provides the same function as the ObservableProperty but it can not change a value directly and can not bind in the two-way direction.

``` csharp
public class Content
{
    public ObservableProperty<bool> IsExecuting { get; } = new ObservableProperty<bool>(false);
    public BoundProperty<bool> CanExecute { get; }

    public Content()
    {
        CanExecute = BoundProperty.By<bool>(IsExecuting, value => !value);
    }
}
```

### EditableContentProperty

The EditableContentProperty provides that a value can be edited with a dedicate content. In order to edit the value with the dedicate content, implementations to edit the value must be added when the EditStarted, EditCompleted, or EditCanceled events occurred.

The classes that can be edited with a special content are as follows.

#### EditableTextProperty

The value is edited with a text value.

#### EditableSelectionProperty

The value is edited with selection items.

## NuGet

[Charites.Bindings](https://www.nuget.org/packages/Charites.Bindings/)

## LICENSE

This software is released under the MIT License, see LICENSE.
