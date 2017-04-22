using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;

namespace Xiaoya.Helpers
{
    public class ParserHelper
    {
        /// <summary>
        /// Get first element from a element collection
        /// </summary>
        /// <param name="elements">Collection of element</param>
        /// <returns><see cref="IElement"/>. Returns <c>null</c> if there is no element.</returns>
        public static IElement GetFirstElement(IHtmlCollection<IElement> elements)
        {
            if (elements.Count() > 0)
            {
                return elements[0];
            } 
            else
            {
                return null;
            }
        }
    }
}
