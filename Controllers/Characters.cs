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
	public class Characters : EntityController<CharacterController> {

		readonly Space space;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		/// <param name="space"></param>
		public Characters ( World world, Space space ) : base(world)
		{
			this.space	=	space;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( float elapsedTime, bool dirty )
		{
			IterateObjects( dirty, (d,e,c) => {

				if (dirty) {

					Move( c, e );

					foreach (var pair in c.Body.CollisionInformation.Pairs) {
						pair.ClearContacts();
					}

					c.SupportFinder.ClearSupportData();

					c.Body.Position			=	MathConverter.Convert( e.Position );
					c.Body.LinearVelocity	=	MathConverter.Convert( e.LinearVelocity );
					c.Body.AngularVelocity	=	MathConverter.Convert( e.AngularVelocity );

				} else {
					Move( c, e );

					e.Position			=	MathConverter.Convert( c.Body.Position ); 
					e.LinearVelocity	=	MathConverter.Convert( c.Body.LinearVelocity );
					e.AngularVelocity	=	MathConverter.Convert( c.Body.AngularVelocity );

					if (c.SupportFinder.HasTraction) {
						e.State |= EntityState.HasTraction;
					} else {
						e.State &= ~EntityState.HasTraction;
					}
				}
			});
		}


		#if false
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( float elapsedTime, bool dirty )
		{
			IterateObjects( dirty, (d,e,c) => {

				var delta	=	MathConverter.Convert( c.Body.LinearVelocity ) * e.Lag;

				if(true) {
					Move( c, e );

					e.Position			= MathConverter.Convert( c.Body.Position ); 
					e.LinearVelocity	= MathConverter.Convert( c.Body.LinearVelocity );
					e.AngularVelocity	= MathConverter.Convert( c.Body.AngularVelocity );
				}

				if (e.RemoteEntity!=null) {

					var dist 	= Vector3.Distance( e.Position, e.RemoteEntity.Position );
					Log.Message("error - {0}", dist );

					if (dist<0.1f) {

						e.RemoteEntity = null;
					} else {
						var re			=	e.RemoteEntity;
						var pos			= Vector3.Lerp(e.Position - delta, re.Position - re.LinearVelocity * re.Lag, 0.5f );
						c.Body.Position = MathConverter.Convert( pos );
					}
				} 
			});
		}
		#endif



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{
			CharacterController controller;
			
			if ( RemoveObject( id, out controller ) ) {
				space.Remove( controller );
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="moveVector"></param>
		void Move ( CharacterController controller, Entity ent )
		{
			var m	=	Matrix.RotationQuaternion( ent.Rotation );

			var move = Vector3.Zero;
			var jump = false;
			if (ent.UserCtrlFlags.HasFlag( UserCtrlFlags.Forward )) move += m.Forward;
			if (ent.UserCtrlFlags.HasFlag( UserCtrlFlags.Backward )) move += m.Backward;
			if (ent.UserCtrlFlags.HasFlag( UserCtrlFlags.StrafeLeft )) move += m.Left;
			if (ent.UserCtrlFlags.HasFlag( UserCtrlFlags.StrafeRight )) move += m.Right;
			if (ent.UserCtrlFlags.HasFlag( UserCtrlFlags.Jump )) jump = true;

			if (controller==null) {
				return;
			}

			controller.HorizontalMotionConstraint.MovementDirection = new BEPUutilities.Vector2( move.X, -move.Z );
			controller.HorizontalMotionConstraint.TargetSpeed	=	8.0f;

			controller.TryToJump = jump;
			/*if (jump) {
				controller.Jump();
			} */
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		public void AddCharacter ( Entity entity, 
			float height = 1.7f, 
			float crouchingHeight = 1.19f, 
			float radius = 0.6f, 
			float margin = 0.1f, 
			float mass = 10f, 
			float maximumTractionSlope = 0.8f, 
			float maximumSupportSlope = 1.3f, 
			float standingSpeed = 8f, 
			float crouchingSpeed = 3f, 
			float tractionForce = 1000f, 
			float slidingSpeed = 6f, 
			float slidingForce = 50f, 
			float airSpeed = 1f, 
			float airForce = 250f, 
			float jumpSpeed = 6f, 
			float slidingJumpSpeed = 3f, 
			float maximumGlueForce = 5000f )
		{
			var pos = MathConverter.Convert( entity.Position );
			var controller = new CharacterController( pos, 
					height					, 
					crouchingHeight			, 
					radius					, 
					margin					, 
					mass					,
					maximumTractionSlope	, 
					maximumSupportSlope		, 
					standingSpeed			,
					crouchingSpeed			,
					tractionForce			, 
					slidingSpeed			,
					slidingForce			,
					airSpeed				,
					airForce				, 
					jumpSpeed				, 
					slidingJumpSpeed		,
					maximumGlueForce		);

			controller.StepManager.MaximumStepHeight	=	0.5f;

			space.Add( controller );

			AddObject( entity.ID, controller );
		}
	}
}
