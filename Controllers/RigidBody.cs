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
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.EntityStateManagement;
using BEPUphysics.PositionUpdating;
//using BEPUphysics.


namespace ShooterDemo.Controllers {
	public class RigidBody : EntityController<Box> {

		readonly Space space;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		/// <param name="space"></param>
		public RigidBody ( World world, Space space ) : base(world)
		{
			this.space	=	space;
		}

		Random rand = new Random();


		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetID"></param>
		/// <param name="attackerID"></param>
		/// <param name="damage"></param>
		/// <param name="kickImpulse"></param>
		/// <param name="kickPoint"></param>
		/// <param name="damageType"></param>
		public override bool Damage ( uint targetID, uint attackerID, short damage, Vector3 kickImpulse, Vector3 kickPoint, DamageType damageType )
		{
			ApplyToObject( targetID, (e,box) => {

				var i = MathConverter.Convert( kickImpulse );
				var p = MathConverter.Convert( kickPoint );
				box.ApplyImpulse( p, i );

				e.SetItemCount(Inventory.Countdown, (short)rand.Next(500,1000));
				
			});

			return false;
		}




		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( float elapsedTime, bool dirty )
		{
			IterateObjects( dirty, (d,e,rb) => {

				if (dirty) {

					foreach (var pair in rb.CollisionInformation.Pairs) {
						pair.ClearContacts();
					}

					rb.Position			=	MathConverter.Convert( e.Position );
					rb.Orientation		=	MathConverter.Convert( e.Rotation );
					rb.LinearVelocity	=	MathConverter.Convert( e.LinearVelocity );
					rb.AngularVelocity	=	MathConverter.Convert( e.AngularVelocity );

				} else {

					if (e.GetItemCount(Inventory.Countdown) > 0) {
						short delta = (short)(elapsedTime*1000);
						var count = e.GetItemCount(Inventory.Countdown);

						if (count<=delta) {
							World.GetController<Projectiles>().Explode( "Explosion", 0, e, e.Position, Vector3.Up, 3, 50, 100, DamageType.RocketExplosion );
							e.SetItemCount(Inventory.Countdown, 0);
							World.Kill( e.ID );
						} else {
							e.ConsumeItem(Inventory.Countdown, delta);
						}
					}

					e.Position			=	MathConverter.Convert( rb.Position ); 
					e.Rotation			=	MathConverter.Convert( rb.Orientation ); 
					e.LinearVelocity	=	MathConverter.Convert( rb.LinearVelocity );
					e.AngularVelocity	=	MathConverter.Convert( rb.AngularVelocity );
				}
			});
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{
			Box obj;
			
			if ( RemoveObject( id, out obj ) ) {
				space.Remove( obj );
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <param name="d"></param>
		/// <param name="mass"></param>
		public void AddBox ( Entity entity, float w, float h, float d, float mass )
		{
			var ms	=	new MotionState();
			ms.AngularVelocity	=	MathConverter.Convert( entity.AngularVelocity );
			ms.LinearVelocity	=	MathConverter.Convert( entity.LinearVelocity );
			ms.Orientation		=	MathConverter.Convert( entity.Rotation );
			ms.Position			=	MathConverter.Convert( entity.Position );
			Box	box	=	new Box(  ms, w, h, d, mass );
			box.PositionUpdateMode	=	PositionUpdateMode.Continuous;

			box.Tag	=	entity;

			AddObject( entity.ID, box );

			space.Add( box );
		}
	}
}
