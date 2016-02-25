using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Engine.Common;


namespace ShooterDemo.Core {
	public interface IEntityController {

		/// <summary>
		/// Updates controller.
		/// </summary>
		/// <param name="gameTime"></param>
		/// <param name="dirty">Entity state is newer than one in controller</param>
		void Update ( float elapsedTime, bool dirty );

		/// <summary>
		/// Called when entity has died.
		/// </summary>
		/// <param name="id"></param>
		void Kill ( uint id );
	}
}
