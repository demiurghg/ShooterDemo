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
using Fusion.Engine.Audio;


namespace ShooterDemo.Views {
	public class CameraView : EntityView<object> {

		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		/// <param name="space"></param>
		public CameraView ( World world ) : base(world)
		{
			if (world.IsClientSide) {
				currentFov	=	(World.GameClient as ShooterClient).Config.Fov;
			}
		}


		float currentFov;
		Vector3 filteredPos = Vector3.Zero;

		/// <summary>
		/// 
		/// </summary>
		public float Sensitivity {
			get {
				var cfg = ((ShooterClient)World.GameClient).Config;
				return currentFov / cfg.Fov * cfg.Sensitivity;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( float elapsedTime, float lerpFactor )
		{
			var rw	= Game.RenderSystem.RenderWorld;
			var sw	= Game.SoundSystem.SoundWorld;
			var vp	= Game.RenderSystem.DisplayBounds;
			var cfg	= ((ShooterClient)World.GameClient).Config;

			var aspect	=	(vp.Width) / (float)vp.Height;
		
			//rw.Camera.SetupCameraFov( new Vector3(10,4,10), new Vector3(0,4,0), Vector3.Up, MathUtil.Rad(90), 0.125f, 1024f, 1, 0, aspect );

			var player	=	World.GetEntityOrNull( e => e.Is("player") && e.UserGuid == World.UserGuid );

			if (player==null) {
				return;
			}


			CalcRecoil( player );
			CalcBobbing( player, elapsedTime );

			var uc	=	(World.GameClient as ShooterClient).UserCommand;

			var m	= 	Matrix.RotationYawPitchRoll(	
							uc.Yaw	 + bobYaw.Offset, 
							uc.Pitch + bobPitch.Offset, 
							uc.Roll	 + bobRoll.Offset
						);

			var ppos	=	player.LerpPosition(lerpFactor);
			//ppos.Y = 4;
			float backoffset = ((ShooterClient)World.GameClient).Config.ThirdPerson ? 2 : 0;
			var pos		=	ppos + Vector3.Up * 1.0f + m.Backward * backoffset;

			/*filteredPos = Vector3.Lerp( filteredPos, pos, 0.5f );
			pos = filteredPos;//*/

			var fwd	=	pos + m.Forward;
			var up	=	m.Up;


			var targetFov	=	MathUtil.Clamp( uc.CtrlFlags.HasFlag( UserCtrlFlags.Zoom ) ? cfg.ZoomFov : cfg.Fov, 10, 140 );

			currentFov		=	MathUtil.Drift( currentFov, targetFov, 360*elapsedTime, 360*elapsedTime );

			rw.Camera.SetupCameraFov( pos, fwd, up, MathUtil.Rad(currentFov), 0.125f, 1024f, 1, 0, aspect );

			sw.Listener	=	new AudioListener();
			sw.Listener.Position	=	pos;
			sw.Listener.Forward		=	m.Forward;
			sw.Listener.Up			=	m.Up;
			sw.Listener.Velocity	=	Vector3.Zero;

			//Vector3 n, p;
			//(World as MPWorld).RayCastAgainstAll( pos, pos + m.Forward * 100, out n, out p );

			//rw.Debug.DrawPoint ( p, 0.5f, Color.Orange );
			//rw.Debug.DrawVector( p, n, Color.Orange );

		}


		Oscillator bobPitch = new Oscillator(100,20);
		Oscillator bobRoll	= new Oscillator( 50,10);
		Oscillator bobYaw	= new Oscillator( 50,10);

		Vector3 oldVelocity = Vector3.Zero;
		bool oldTraction = true;

		class Oscillator {

			public float Offset { get { return offset; } }
			public float Target = 0;

			readonly float damping;
			readonly float stiffness;
			float offset	=	0;
			float velocity	=	0;
			const float mass = 1;

			public Oscillator ( float stiffness, float damping )
			{
				this.stiffness	=	stiffness;
				this.damping	=	damping;
			}

			public void Kick ( float velocity )
			{
				this.velocity	=	velocity;
			}


			public void Update ( float elapsed )
			{
				float force =	(Target-offset) * stiffness - velocity * damping;
				velocity	=	velocity + (force/mass) * elapsed;
				offset		=	offset + velocity * elapsed;
			}
		}



		float stepCounter;
		bool rlStep;

		Random rand = new Random();


		/// <summary>
		/// 
		/// </summary>
		/// <param name="player"></param>
		void CalcRecoil ( Entity player )
		{
			if (player.State.HasFlag( EntityState.WeaponRecoilLight )) {
				bobPitch.Kick( rand.NextFloat(-0.25f,0.25f) );
				bobYaw  .Kick( rand.NextFloat(-0.25f,0.25f) );
				bobRoll .Kick( rand.NextFloat(-0.25f,0.25f) );
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		void CalcBobbing ( Entity player, float elapsedTime )
		{	
			var clientCfg	=	((ShooterClient)World.GameClient).Config;

			bool hasTraction	=	player.State.HasFlag(EntityState.HasTraction);	

			if (hasTraction && !oldTraction) {
				bobPitch.Kick( -clientCfg.BobLand * Math.Abs(oldVelocity.Y/10.0f) );

				if (oldVelocity.Y<-10) {
					World.RunFX( "PlayerLanding", 0, player.Position );
				} else {
					World.RunFX( "PlayerFootStep", 0, player.Position );
				}
			}

			oldVelocity	=	player.LinearVelocity;
			oldTraction	=	hasTraction;

			if (player.UserCtrlFlags.HasFlag(UserCtrlFlags.StrafeRight)
			||	player.UserCtrlFlags.HasFlag(UserCtrlFlags.StrafeLeft)
			||	player.UserCtrlFlags.HasFlag(UserCtrlFlags.Forward)	
			||	player.UserCtrlFlags.HasFlag(UserCtrlFlags.Backward) ) {

				stepCounter -= elapsedTime;

				if (stepCounter<0) {
					stepCounter = 0.30f;


					rlStep = !rlStep;

					if (hasTraction) {
						World.RunFX( "PlayerFootStep", 0, player.Position );
						bobRoll.Kick( (rlStep ? 1 : -1) * clientCfg.BobRoll );
						bobPitch.Kick( clientCfg.BobPitch );
					}
				}
			}

			//
			//	strafe rolling :
			//
			var rollPull = 0.0f;

			if (hasTraction) {
				if (player.UserCtrlFlags.HasFlag(UserCtrlFlags.StrafeRight)) {
					rollPull	-=	MathUtil.DegreesToRadians( clientCfg.BobStrafe );
				} 
				if (player.UserCtrlFlags.HasFlag(UserCtrlFlags.StrafeLeft)) {
					rollPull	+=	MathUtil.DegreesToRadians( clientCfg.BobStrafe );
				} 
			}

			bobRoll.Target = rollPull;


			bobRoll.Update( elapsedTime );
			bobPitch.Update( elapsedTime );
			bobYaw.Update( elapsedTime );

			
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{
			/*object controller;
			
			if ( RemoveObject( id, out controller ) ) {
				space.Remove( controller );
			} */
		}
	}
}
