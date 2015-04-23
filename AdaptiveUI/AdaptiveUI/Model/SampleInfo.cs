using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace AdaptiveUI.Model
{
    /// <summary>
    /// An entity class for representing a sample.
    /// </summary>
    public class SampleInfo
    {
        /// <summary>
        /// Gets or sets an icon for the sample.
        /// </summary>
        public IconElement Icon { get; set; }

        /// <summary>
        /// Gets or sets a title for the sample.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type of the sample
        /// </summary>
        public Type Type { get; set; }
    }
}
