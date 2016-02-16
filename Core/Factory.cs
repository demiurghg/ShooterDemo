using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Core.Mathematics;

namespace ShooterDemo.Core {
	public class Factory {

		/// <summary>
		/// Creates entity
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="parent"></param>
		/// <param name="world"></param>
		/// <returns></returns>
		public Entity Construct ( string prefab, Entity parent, Matrix world )
		{
			return null;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="createAction"></param>
		public void DefinePrefab ( string prefab, Action createAction )
		{
		}
	}
}
