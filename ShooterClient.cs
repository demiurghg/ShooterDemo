using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fusion;
using Fusion.Core;
using Fusion.Core.Mathematics;
using Fusion.Core.Configuration;
using Fusion.Engine.Common;
using Fusion.Engine.Input;
using Fusion.Engine.Client;
using Fusion.Engine.Server;
using Fusion.Engine.Graphics;
using ShooterDemo.Core;
using ShooterDemo.Views;


namespace ShooterDemo {
	public class ShooterClient : Fusion.Engine.Client.GameClient {

		World gameWorld;

		public World World {
			get { return gameWorld; }
		}


		[Config]
		public ShooterClientConfig Config { get; set; }


		SpriteLayer	hudLayer;

		public SpriteLayer HudLayer {
			get { return hudLayer; }
		}


		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="engine"></param>
		public ShooterClient ( Game game )
			: base( game )
		{
			this.Config	=	new ShooterClientConfig();
		}



		/// <summary>
		/// Initializes game
		/// </summary>
		public override void Initialize ()
		{
			hudLayer	=	new SpriteLayer( Game.RenderSystem, 1024 );
			Game.RenderSystem.RenderWorld.SpriteLayers.Add( hudLayer );

			Game.Keyboard.KeyDown += Keyboard_KeyDown;
		}


		protected override void Dispose ( bool disposing )
		{
			if (disposing) {
				SafeDispose( ref hudLayer );
			}
			base.Dispose( disposing );
		}


		/// <summary>
		/// Called when connection request accepted by server.
		/// Client could start loading models, textures, models etc.
		/// </summary>
		/// <param name="map"></param>
		public override GameLoader LoadContent ( string serverInfo )
		{
			latestSnapshot	=	null;

			Log.Message("");
			Log.Message("---- Loading game: {0} ----", serverInfo );

			return new ShooterLoader( this, serverInfo );
		}


		/// <summary>
		/// Called when loader finished loading.
		/// This method lets client to complete loading process in main thread.
		/// </summary>
		/// <param name="loader"></param>
		public override void FinalizeLoad ( GameLoader loader )
		{
			var shooterLoader = (ShooterLoader)loader;

			gameWorld = shooterLoader.World;

			gameWorld.FinalizeLoad();

			gameWorld.ReplicaSpawned += (s,e) => { 
				UserCommand.Yaw			=	0;
				UserCommand.Pitch		=	0;
				UserCommand.Roll		=	0;
				UserCommand.CtrlFlags	=	UserCtrlFlags.None;
			};


			var rw = Game.RenderSystem.RenderWorld;

			rw.HdrSettings.BloomAmount	= 0.1f;
			rw.HdrSettings.DirtAmount	= 0.0f;
			rw.HdrSettings.KeyValue		= 0.18f;

			rw.SkySettings.SunPosition			=	new Vector3(1,2,1);
			rw.SkySettings.SunLightIntensity	=	100;
			rw.SkySettings.SkyTurbidity			=	8;

			rw.LightSet.DirectLight.Direction	=	rw.SkySettings.SunLightDirection;
			rw.LightSet.DirectLight.Intensity	=	rw.SkySettings.SunLightColor;

			rw.LightSet.EnvLights.Add( new EnvLight( new Vector3(0,4,0), 1, 500 ) );
			for (float x=-32; x<=32; x+=16 ) {
				for (float y=-32; y<=32; y+=16 ) {
					rw.LightSet.EnvLights.Add( new EnvLight( new Vector3(x,4,y), 1, 16 ) );
				}
			}

			Log.Message("Capturing radiance...");
			rw.RenderRadiance();

			Game.GetModule<ShooterInterface>().ShowMenu = false;

			Log.Message("---- Loading game completed ----");
			Log.Message("");
		}



		/// <summary>
		///	Called when client disconnected, dropped, kicked or timeouted.
		///	Client must purge all level-associated content.
		///	Reason???
		/// </summary>
		public override void UnloadContent ()
		{
			latestSnapshot	=	null;

			if (World!=null) {
				World.Cleanup();
			}
			Game.RenderSystem.RenderWorld.ClearWorld();

			Content.Unload();
			Game.GetModule<ShooterInterface>().ShowMenu = true;
		}



		public UserCommand	UserCommand;

		uint ackCommandID;
		byte[] latestSnapshot = null;

		public float entityLerpFactor {
			get;
			private set;
		}


		/// <summary>
		/// Runs one step of client-side simulation and renders world state.
		/// Do not close the stream.
		/// </summary>
		/// <param name="gameTime"></param>
		public override byte[] Update ( GameTime gameTime, uint sentCommandID )
		{
			// update user input :
			UpdateInput( ref UserCommand );
			var cmdBytes = UserCommand.GetBytes( UserCommand );

			//	process incoming snapshots :
			ProcessSnapshot(gameTime);

			//	movement prediction :
			//gameWorld.PlayerCommand( this.Guid, cmdBytes, 0 );
			//gameWorld.SimulateWorld( gameTime.ElapsedSec );

			//	render world :
			gameWorld.PresentWorld( gameTime.ElapsedSec, entityLerpFactor );

			return cmdBytes;
		}



		GameTime serverTime;

		Core.Filter filter = new Core.Filter(8);


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ProcessSnapshot ( GameTime gameTime )
		{
			//	calc lerp factor :
			float lerpInc	=	1;
			
			if (serverTime.ElapsedSec!=0) {
				filter.Push(gameTime.ElapsedSec / serverTime.ElapsedSec);
				lerpInc  = filter.Value;
			}

			entityLerpFactor += lerpInc;

			// snapshot has arrived :
			if (latestSnapshot!=null) {

				//bool late = false;
				bool early = false;

				//	check whether chanpshot is too early or late :
				if ( !MathUtil.WithinEpsilon( entityLerpFactor, 1, lerpInc/2.0f ) ) {
					if (entityLerpFactor>1) {
						//late = true;
					}
					if (entityLerpFactor<1) {
						early = true;
					}
				}

				//	snapshot too early, wait next frame.
				if (early) {
					return false;
				}

				//	read snapshot :
				using ( var ms = new MemoryStream(latestSnapshot) ) {
					using ( var reader = new BinaryReader(ms) ) {

						reader.ReadSingle();

						gameWorld.ReadFromSnapshot( reader, ackCommandID, entityLerpFactor );
					
						entityLerpFactor = 0;
					}
				}

				latestSnapshot	=	null;

				return true;

			} else {
				return false;
			}
		}




		void Keyboard_KeyDown ( object sender, KeyEventArgs e )
		{
			if (e.Key==Config.UseWeapon1) {	
				UserCommand.CtrlFlags &= ~UserCtrlFlags.AllWeapon;
				UserCommand.CtrlFlags |= UserCtrlFlags.Machinegun;
			}
			if (e.Key==Config.UseWeapon2) {	
				UserCommand.CtrlFlags &= ~UserCtrlFlags.AllWeapon;
				UserCommand.CtrlFlags |= UserCtrlFlags.Shotgun;
			}
			if (e.Key==Config.UseWeapon3) {	
				UserCommand.CtrlFlags &= ~UserCtrlFlags.AllWeapon;
				UserCommand.CtrlFlags |= UserCtrlFlags.SuperShotgun;
			}
			if (e.Key==Config.UseWeapon4) {	
				UserCommand.CtrlFlags &= ~UserCtrlFlags.AllWeapon;
				UserCommand.CtrlFlags |= UserCtrlFlags.GrenadeLauncher;
			}
			if (e.Key==Config.UseWeapon5) {	
				UserCommand.CtrlFlags &= ~UserCtrlFlags.AllWeapon;
				UserCommand.CtrlFlags |= UserCtrlFlags.RocketLauncher;
			}
			if (e.Key==Config.UseWeapon6) {	
				UserCommand.CtrlFlags &= ~UserCtrlFlags.AllWeapon;
				UserCommand.CtrlFlags |= UserCtrlFlags.Machinegun;
			}
			if (e.Key==Config.UseWeapon7) {	
				UserCommand.CtrlFlags &= ~UserCtrlFlags.AllWeapon;
				UserCommand.CtrlFlags |= UserCtrlFlags.Railgun;
			}
			if (e.Key==Config.UseWeapon8) {	
				UserCommand.CtrlFlags &= ~UserCtrlFlags.AllWeapon;
				UserCommand.CtrlFlags |= UserCtrlFlags.HyperBlaster;
			}
			if (e.Key==Config.UseWeapon9) {	
				UserCommand.CtrlFlags &= ~UserCtrlFlags.AllWeapon;
				UserCommand.CtrlFlags |= UserCtrlFlags.BFG;
			}
		}



		/// <summary>
		/// 
		/// </summary>
		void UpdateInput ( ref UserCommand userCommand )
		{
			var flags = UserCtrlFlags.None;
			var cfg	  = Config;
			
			if (Game.Keyboard.IsKeyDown( cfg.MoveForward	)) flags |= UserCtrlFlags.Forward;
			if (Game.Keyboard.IsKeyDown( cfg.MoveBackward	)) flags |= UserCtrlFlags.Backward;
			if (Game.Keyboard.IsKeyDown( cfg.StrafeLeft		)) flags |= UserCtrlFlags.StrafeLeft;
			if (Game.Keyboard.IsKeyDown( cfg.StrafeRight	)) flags |= UserCtrlFlags.StrafeRight;
			if (Game.Keyboard.IsKeyDown( cfg.Jump			)) flags |= UserCtrlFlags.Jump;
			if (Game.Keyboard.IsKeyDown( cfg.Crouch			)) flags |= UserCtrlFlags.Crouch;
			if (Game.Keyboard.IsKeyDown( cfg.Zoom			)) flags |= UserCtrlFlags.Zoom;
			if (Game.Keyboard.IsKeyDown( cfg.Attack			)) flags |= UserCtrlFlags.Attack;


			//	http://eliteownage.com/mousesensitivity.html 
			//	Q3A: 16200 dot per 360 turn:
			var vp		=	Game.RenderSystem.DisplayBounds;
			var ui		=	Game.GameInterface as ShooterInterface;
			var cam		=	World.GetView<CameraView>();

			if (!ui.Console.Show) {
				UserCommand.CtrlFlags	=	flags;
				UserCommand.Yaw			-=	2 * MathUtil.Pi * cam.Sensitivity * Game.Mouse.PositionDelta.X / 16200.0f;
				UserCommand.Pitch		-=	2 * MathUtil.Pi * cam.Sensitivity * Game.Mouse.PositionDelta.Y / 16200.0f * ( Config.InvertMouse ? -1 : 1 );
				UserCommand.Roll		=	0;
			}
		}



		/// <summary>
		/// Feed server snapshot to client.
		/// Called when fresh snapshot arrived.
		/// </summary>
		/// <param name="snapshot"></param>
		public override void FeedSnapshot ( GameTime serverTime, byte[] snapshot, uint ackCommandID )
		{
			this.serverTime		=	serverTime;

			this.ackCommandID	=	ackCommandID;
			this.latestSnapshot	=	snapshot;
		}



		/// <summary>
		/// Feed server notification to client.
		/// </summary>
		/// <param name="snapshot"></param>
		public override void FeedNotification ( string message )
		{
			Log.Message( "NOTIFICATION : {0}", message );
		}



		/// <summary>
		/// Returns user informations.
		/// </summary>
		/// <returns></returns>
		public override string UserInfo ()
		{
			return "Bob" + System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
		}



		/// <summary>
		/// 
		/// </summary>
		public void PrintState ()
		{
			
		}
	}
}
