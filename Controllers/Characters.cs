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
	public class Characters : EntityController<Characters.Character> {

		readonly Space space;


		const float StepRate = 0.3f;


		public class Character {

			public Character ( CharacterController controller ) {
				this.Controller = controller;
			}
			
			readonly public CharacterController Controller;
			public float StepCounter;
			public bool	 RLStep;
			public bool	 OldTraction = true;
			public Vector3 OldVelocity = Vector3.Zero;
		}


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
		/// <param name="targetID"></param>
		/// <param name="attackerID"></param>
		/// <param name="damage"></param>
		/// <param name="kickImpulse"></param>
		/// <param name="kickPoint"></param>
		/// <param name="damageType"></param>
		public override bool Damage ( uint targetID, uint attackerID, short damage, Vector3 kickImpulse, Vector3 kickPoint, DamageType damageType )
		{
			ApplyToObject( targetID, (e,ch) => {

				var c = ch.Controller;

				c.SupportFinder.ClearSupportData();
				var i = MathConverter.Convert( kickImpulse );
				var p = MathConverter.Convert( kickPoint );
				c.Body.ApplyImpulse( p, i );


				//
				//	calc health :
				//
				var health	=	e.GetItemCount( Inventory.Health );
				health -= damage;

				if (health<0) {
					World.Kill( targetID );
				}

				e.SetItemCount( Inventory.Health, health );
				
			});

			return false;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( float elapsedTime, bool dirty )
		{
			IterateObjects( dirty, (d,e,ch) => {

				var c = ch.Controller;

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

				UpdateWalkSFX( e, ch, elapsedTime );
				UpdateFallSFX( e, ch, elapsedTime );
			});
		}



		void UpdateWalkSFX ( Entity e, Character ch, float elapsedTime )
		{					
			ch.StepCounter -= elapsedTime;
			if (ch.StepCounter<=0) {
				ch.StepCounter = StepRate;
				ch.RLStep = !ch.RLStep;

				bool step	=	e.UserCtrlFlags.HasFlag( UserCtrlFlags.Forward )
							|	e.UserCtrlFlags.HasFlag( UserCtrlFlags.Backward )
							|	e.UserCtrlFlags.HasFlag( UserCtrlFlags.StrafeLeft )
							|	e.UserCtrlFlags.HasFlag( UserCtrlFlags.StrafeRight );

				if (step && ch.Controller.SupportFinder.HasTraction) {
					if (ch.RLStep) {
						World.SpawnFX("PlayerFootStepR", e.ID, e.Position );
					} else {
						World.SpawnFX("PlayerFootStepL", e.ID, e.Position );
					}
				}
			}
		}



		void UpdateFallSFX ( Entity e, Character ch, float elapsedTime )
		{
			bool newTraction = ch.Controller.SupportFinder.HasTraction;
			
			if (ch.OldTraction!=newTraction && newTraction) {
				if (((ShooterServer)World.GameServer).Config.ShowFallings) {
					Log.Verbose("{0} falls : {1}", e.ID, ch.OldVelocity.Y );
				}

				if (ch.OldVelocity.Y<-10) {
					//	medium landing :
					World.SpawnFX( "PlayerLanding", e.ID, e.Position, ch.OldVelocity, Quaternion.Identity );
				} else {
					//	light landing :
					World.SpawnFX( "PlayerFootStepL", e.ID, e.Position );
				}
			}

			ch.OldTraction = newTraction;
			ch.OldVelocity = MathConverter.Convert(ch.Controller.Body.LinearVelocity);
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
			Character character;
			
			if ( RemoveObject( id, out character ) ) {
				space.Remove( character.Controller );
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
			
			if (jump && controller.StanceManager.CurrentStance!=Stance.Crouching) {
				if (controller.SupportFinder.HasSupport || controller.SupportFinder.HasTraction) {
					World.SpawnFX( "PlayerJump", ent.ID, ent.Position );
				}
			}
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
			controller.Body.Tag	=	entity;
			controller.Tag		=	entity;

			space.Add( controller );

			AddObject( entity.ID, new Character(controller) );
		}
	}
}
