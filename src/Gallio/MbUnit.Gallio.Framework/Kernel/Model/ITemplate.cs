// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.DataBinding;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// <para>
    /// A template is an abstract parameterized description of some part of a test.
    /// </para>
    /// <para>
    /// During test enumeration, a tree of templates is constructed to
    /// describe the test assemblies, fixtures, methods, and other entities
    /// that make up the test.  The tree reflects the logical structure
    /// in which the tests were defined.  Typically the tree will be
    /// traversed when building the final test graph during test enumeration
    /// but the order in which tests are executed is not required to
    /// correspond to the hierarchical arrangement of templates in the tree.
    /// </para>
    /// <para>
    /// A template may have zero or more <see cref="ITemplateParameter" />s.
    /// Values must be bound to each parameter to produce a fully-bound
    /// <see cref="ITemplateBinding" /> from which tests can be built.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// During test enumeration, the template contributes <see cref="ITest" />s to a
    /// <see cref="TestTreeBuilder" />.  The template can inject its own contributions
    /// in and around those of the templates by modifying the <see cref="TemplateBindingScope" />
    /// it passes to the inner templates.
    /// </para>
    /// <para>
    /// It can happen that a template is never asked to produce tests, possibly because
    /// it has some other purpose, or because no values were specified for its parameters
    /// in the enclosing context.  In this case, the template does not contribute anything
    /// to the test tree but it will still be visible in the template tree.
    /// </para>
    /// </remarks>
    public interface ITemplate : ITemplateComponent, IModelTreeNode<ITemplate>
    {
        /// <summary>
        /// Gets the parameter that belong to this template.
        /// Each parameter must have a unique name.  The order in which
        /// the parameters appear is not significant.
        /// </summary>
        IList<ITemplateParameter> Parameters { get; }

        /// <summary>
        /// Binds a template to a particular scope with the specified arguments.
        /// </summary>
        /// <remarks>
        /// The template has a chance to validate or transform the arguments
        /// as the binding is created.
        /// </remarks>
        /// <param name="scope">The scope in which the template was bound</param>
        /// <param name="arguments">The actual values for the template's parameters</param>
        /// <returns>The template binding</returns>
        ITemplateBinding Bind(TemplateBindingScope scope, IDictionary<ITemplateParameter, IDataFactory> arguments);
    }
}
