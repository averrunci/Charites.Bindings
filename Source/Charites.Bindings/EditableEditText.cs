﻿// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;

namespace Charites.Windows.Mvc.Bindings
{
    internal sealed class EditableEditText : EditableEditContent<string>, IEditableEditText
    {
        public ObservableProperty<bool> IsMultiLine { get; }

        private Func<string, bool> Validator { get; }

        public EditableEditText(Func<string, bool> validator, bool multiLine)
        {
            Validator = validator;
            IsMultiLine = multiLine.ToObservableProperty();
        }

        public bool Validate() => Validator?.Invoke(Value.Value) ?? true;
    }
}
