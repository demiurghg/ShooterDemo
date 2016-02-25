using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Engine.Common;


namespace ShooterDemo.Core {
	public interface IEntityView {

		/// <summary>
		/// Called on each viewable entity.
		/// </summary>
		/// <param name="entity"></param>
		void Update ( float elapsedTime );

		/// <summary>
		/// Called when entity has died.
		/// </summary>
		/// <param name="id"></param>
		void Kill ( uint id );
	}
}
