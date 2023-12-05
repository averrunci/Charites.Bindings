// Copyright (C) 2022-2023 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Specification(
    "ObservableProperty Spec",
    typeof(ObservablePropertySpec_InstanceCreation),
    typeof(ObservablePropertySpec_PropertyChanged),
    typeof(ObservablePropertySpec_Binding),
    typeof(ObservablePropertySpec_ValueChangeHandling),
    typeof(ObservablePropertySpec_Validation),
    typeof(ObservablePropertySpec_DelayValueChange)
)]
class ObservablePropertySpec;