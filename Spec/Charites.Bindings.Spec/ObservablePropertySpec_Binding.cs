// Copyright (C) 2022-2023 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context(
    "Binding",
    typeof(ObservablePropertySpec_Binding_OneWay),
    typeof(ObservablePropertySpec_Binding_OneWayWithConverter),
    typeof(ObservablePropertySpec_Binding_MultiBinding),
    typeof(ObservablePropertySpec_Binding_TwoWay),
    typeof(ObservablePropertySpec_Binding_TwoWayWithConverter)
)]
class ObservablePropertySpec_Binding;