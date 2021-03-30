# Release note

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