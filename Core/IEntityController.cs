using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Core.Mathematics;
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetID"></param>
		/// <param name="attackerID"></param>
		/// <param name="damage"></param>
		/// <param name="kickImpulse"></param>
		/// <param name="kickPoint"></param>
		/// <param name="damageType"></param>
		/// <returns>Indicates whether damage was fatal</returns>
		bool Damage ( uint targetID, uint attackerID, short damage, Vector3 kickImpulse, Vector3 kickPoint, DamageType damageType );
	}
}
