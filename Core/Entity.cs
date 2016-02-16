using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShooterDemo.Core {
	public class Entity {
		
		/// <summary>
		///	Gets uniques entity's ID.
		/// </summary>
		public uint UniqueID { get; private set; }

		/// <summary>
		///	Gets parent's ID.
		/// </summary>
		public uint ParentID { get; private set; }

		/// <summary>
		/// Gets prefab ID.
		/// </summary>
		public uint PrefabID { get; private set; }
	}
}
