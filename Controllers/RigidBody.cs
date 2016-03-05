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

			AddObject( entity.ID, box );

			space.Add( box );
		}
	}
}
