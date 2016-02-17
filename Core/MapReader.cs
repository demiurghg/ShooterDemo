using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Core.Content;

namespace ShooterDemo.Core {
	public abstract class MapReader {

		/// <summary>
		/// Reads map.
		/// </summary>
		/// <param name="content"></param>
		/// <param name="assetPath"></param>
		public abstract void ReadMap ( ContentManager content, string assetPath );
	}
}
