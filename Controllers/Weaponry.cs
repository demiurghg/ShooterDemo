using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fusion;
using Fusion.Core;
using Fusion.Core.Content;
using Fusion.Core.Mathematics;
using Fusion.Engine.Common;
using Fusion.Engine.Input;
using Fusion.Engine.Client;
using Fusion.Engine.Server;
using Fusion.Engine.Graphics;
using ShooterDemo.Core;
using BEPUphysics;
using BEPUphysics.Character;


namespace ShooterDemo.Controllers {

	/// <summary>
	/// Player's weaponry controller.
	/// </summary>
	public class Weaponry : EntityController<object> {

		Random	rand	=	new Random();


		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		/// <param name="space"></param>
		public Weaponry ( World world ) : base(world)
		{
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( float elapsedTime, bool dirty )
		{
			this.IterateObjects( false, (d,e,o) => {
				UpdateWeaponState( e, (short)(elapsedTime*1000) );
			});
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		public void Attach ( Entity entity )
		{
			AddObject( entity.ID, null );

			entity.ActiveItem	=	Inventory.Machinegun;
			entity.SetItemCount( Inventory.Bullets		,	150	);
			entity.SetItemCount( Inventory.Machinegun	,	1	);

		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{
			object obj;
			RemoveObject( id, out obj );
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="deltaTime"></param>
		void UpdateWeaponState ( Entity entity, short deltaTime )
		{
			var attack		=	entity.UserCtrlFlags.HasFlag( UserCtrlFlags.Attack );
			var cooldown	=	entity.GetItemCount( Inventory.WeaponCooldown );

			cooldown		=   (short)Math.Max( 0, cooldown - deltaTime );

			entity.SetItemCount( Inventory.WeaponCooldown, cooldown );

			//	weapon is too hot :
			if (cooldown>0) {
				return;
			}

			if (entity.UserCtrlFlags.HasFlag(UserCtrlFlags.Machinegun		)) entity.ActiveItem = Inventory.Machinegun		;
			if (entity.UserCtrlFlags.HasFlag(UserCtrlFlags.Shotgun			)) entity.ActiveItem = Inventory.Shotgun		;
			if (entity.UserCtrlFlags.HasFlag(UserCtrlFlags.SuperShotgun		)) entity.ActiveItem = Inventory.SuperShotgun	;
			if (entity.UserCtrlFlags.HasFlag(UserCtrlFlags.GrenadeLauncher	)) entity.ActiveItem = Inventory.GrenadeLauncher;
			if (entity.UserCtrlFlags.HasFlag(UserCtrlFlags.RocketLauncher	)) entity.ActiveItem = Inventory.RocketLauncher	;
			if (entity.UserCtrlFlags.HasFlag(UserCtrlFlags.HyperBlaster		)) entity.ActiveItem = Inventory.HyperBlaster	;
			if (entity.UserCtrlFlags.HasFlag(UserCtrlFlags.Chaingun			)) entity.ActiveItem = Inventory.Chaingun		;
			if (entity.UserCtrlFlags.HasFlag(UserCtrlFlags.Railgun			)) entity.ActiveItem = Inventory.Railgun		;
			if (entity.UserCtrlFlags.HasFlag(UserCtrlFlags.BFG				)) entity.ActiveItem = Inventory.BFG			;

			var world = (MPWorld)World;

			if (attack) {
				switch (entity.ActiveItem) {
					case Inventory.Machinegun		:	FireBullet(world, entity, 5, 5, 75); break;
					case Inventory.Shotgun			:	break;
					case Inventory.SuperShotgun		:	break;
					case Inventory.GrenadeLauncher	:	break;
					case Inventory.RocketLauncher	:	break;
					case Inventory.HyperBlaster		:	break;
					case Inventory.Chaingun			:	break;
					case Inventory.Railgun			:	break;
					case Inventory.BFG				:	break;
					default: 
						entity.ActiveItem = Inventory.Machinegun;
						break;
				}
			}
		}



		Vector3 AttackPos ( Entity e )
		{
			return e.Position + Vector3.Up;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="damage"></param>
		void FireBullet ( MPWorld world, Entity attacker, int damage, float impulse, short cooldown )
		{
			if (!attacker.ConsumeItem( Inventory.Bullets, 1 )) {
				return;
			}

			var view	=	Matrix.RotationQuaternion( attacker.Rotation );
			Vector3 n,p;
			Entity e;

			var direction	=	view.Forward + rand.UniformRadialDistribution(0, 0.02f);
			var origin		=	AttackPos( attacker );

			if (world.RayCastAgainstAll( origin, origin + direction * 400, out n, out p, out e, attacker )) {

				world.SpawnFX( FXEventType.BulletTrail, origin, p, n );

				if (e!=null) {
					e.Kick( view.Forward * impulse, p );
				}
			}

			attacker.SetItemCount( Inventory.WeaponCooldown, cooldown );
		}





	}
}
