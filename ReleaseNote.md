# Release note

## v2.1.0

### Changes

- Change the specification of the GetValueAt method of the MultiBindingContext class as follows:
  - Change the return value to T from T?.
  - Change to throw an exception when the specified index is outside the range of valid indexes for the binding sources.
  - Change to throw an exception when the specified type is not a valid type of the BindableProperty&lt;T&gt; at the specified index.

## v2.0.0

### Add

- Add a constructor whose parameter is the BindableProperty&lt;T&gt; to the BoundProperty.
- Add the following static factory methods to the BoundProperty:
  - By(BindableProperty&lt;T&gt; source)
  - By&lt;TSource&gt;(BindableProperty&lt;TSource&gt; source, Func&lt;TSource, T&gt; converter)
  - By(Func&lt;MultiBindingContext, T&gt; converter, params INotifyPropertyChanged[] sources)

### Changes

- Update the target framework version to .NET 6.0.
- Enable Nullable reference types.
- Delete a default constructor from the followings:
  - BindableProperty
  - BoundProperty
  - EditableDisplayContent
  - EditableEditContent
  - ObservableProperty
- Delete a constructor whose parameter is the IEnumerable&lt;T&gt; from the EditableSelectionProperty.

## v1.2.1

### Changes

- Change to be able to get its value when to set the BoundProperty at a multi binding.

## v1.2.0

### Add

- Add the BindableProperty class as the base class of the ObservableProperty class.
- Add the BoundProperty class that can bind another BindableProperty and whose value can not be changed directly.
- Add the EnableDelayValueChange and DisableDelayValueChange methods that enable/disable a delay of a property value change.

### Changes

- Change the Bind, Unbind, BindTwoWay, UnbindTwoWay, and EnsureValidation methods to return this.

## v1.1.0

### Changes

- Change the reference to the PropertyChanged event handler from a weak reference to a strong reference.