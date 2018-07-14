## Overview

Charites.Bindings is a class library that contains properties for data bindings. They are mainly two properties as follows.

### ObservableProperty

The ObservableProperty provides notifications when a value is changed and validations of a value.

The ObservableProperty implements the INotifyPropertyChanged interface and raises the PropertyChanged event when the Value property is changed. This property can be used for data bindings. For example in UWP, a model code is as follows:

```
public class User
{
    public ObservableProperty<string> Name { get; } = string.Empty.ToObservableProperty();
    public ObservableProperty<string> Address { get; } = string.Empty.ToObservableProperty();
}
```

When this model is set to a data context, XAML code is as follows:

```
<TextBlock Text="{Binding Name.Value}" />
<TextBlock Text="{Binding Address.Value}" />
```

The ObservableProperty can also bind another ObservableProperty as follows:

```
public class Content
{
    public ObservableProperty<bool> IsExecuting { get; } = new ObservableProperty<bool>();
    public ObservableProperty<bool> CanExecute { get; } = new ObservableProperty<bool>();

    public Content()
    {
        CanExecute.Bind(IsExecuting, value => !value);
    }
}
```

Multiple ObservableProperty can be bound as follows:

```
public class Content
{
    public ObservableProperty<int> Red { get; } = new ObservableProperty<int>();
    public ObservableProperty<int> Green { get; } = new ObservableProperty<int>();
    public ObservableProperty<int> Blue { get; } = new ObservableProperty<int>();
    public ObservableProperty<Color> Color { get; } = new ObservableProperty<Color>();

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

```
public class Content
{
    public ObservableProperty<int> Age { get; } = new ObservableProperty<int>();
    public ObservableProperty<string> InputAge { get; } = new ObservableProperty<string>();

    public Content()
    {
        Age.BindTwoWay(InputAge, int.Parse, value => value.ToString())
    }
}
```

In order that the ObservableProperty implements IDataErrorInfo and INotifyDataErrorInfo interfaces, the value of the ObservableProperty can also be validated using the ValidationAttribute as follows:

```
public class User
{
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "Please enter {0}.")]
    [StringLength(25, ErrorMessage = "Please enter {0} within {1} characters.")]
    public ObservableProperty<string> FirstName { get; } = new ObservableProperty<string>();

    public User()
    {
        FirstName.EnableValidation(() => FirstName);
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
