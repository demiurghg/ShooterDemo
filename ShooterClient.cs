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


namespace ShooterDemo {
	class ShooterClient : Fusion.Engine.Client.GameClient {

		World gameWorld;

		public World World {
			get { return gameWorld; }
		}


		[Config]
		public ShooterClientConfig Config { get; set; }


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
				UserCommand.Yaw			=	e.Entity.Angles.Yaw.Radians;
				UserCommand.Pitch		=	e.Entity.Angles.Pitch.Radians;
				UserCommand.Roll		=	e.Entity.Angles.Roll.Radians;
				UserCommand.CtrlFlags	=	UserCtrlFlags.None;
			};


			var rw = Game.RenderSystem.RenderWorld;

			rw.HdrSettings.BloomAmount	= 0.1f;
			rw.HdrSettings.DirtAmount	= 0.0f;
			rw.HdrSettings.KeyValue		= 0.18f;

			rw.SkySettings.SunPosition			=	Vector3.One;
			rw.SkySettings.SunLightIntensity	=	100;

			rw.LightSet.DirectLight.Direction	=	rw.SkySettings.SunLightDirection;
			rw.LightSet.DirectLight.Intensity	=	rw.SkySettings.SunLightColor;
			rw.LightSet.EnvLights.Add( new EnvLight( new Vector3(0,4,0), 1, 500 ) );

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

			Game.RenderSystem.RenderWorld.ClearWorld();

			Content.Unload();
			Game.GetModule<ShooterInterface>().ShowMenu = true;
		}



		public UserCommand	UserCommand;

		uint ackCommandID;
		byte[] latestSnapshot = null;


		/// <summary>
		/// Runs one step of client-side simulation and renders world state.
		/// Do not close the stream.
		/// </summary>
		/// <param name="gameTime"></param>
		public override byte[] Update ( GameTime gameTime, uint sentCommandID )
		{
			var flags = UserCtrlFlags.None;
			
			if (Game.Keyboard.IsKeyDown( Keys.S				)) flags |= UserCtrlFlags.Forward;
			if (Game.Keyboard.IsKeyDown( Keys.Z				)) flags |= UserCtrlFlags.Backward;
			if (Game.Keyboard.IsKeyDown( Keys.A				)) flags |= UserCtrlFlags.StrafeLeft;
			if (Game.Keyboard.IsKeyDown( Keys.X				)) flags |= UserCtrlFlags.StrafeRight;
			if (Game.Keyboard.IsKeyDown( Keys.RightButton	)) flags |= UserCtrlFlags.Jump;
			if (Game.Keyboard.IsKeyDown( Keys.LeftAlt		)) flags |= UserCtrlFlags.Crouch;

			//	http://eliteownage.com/mousesensitivity.html 
			//	Q3A: 16200 dot per 360 turn:
			var vp		=	Game.RenderSystem.DisplayBounds;
			var ui		=	Game.GameInterface as ShooterInterface;

			if (!ui.Console.Show) {
				UserCommand.CtrlFlags	=	flags;
				UserCommand.Yaw			-=	2 * MathUtil.Pi * Config.Sensitivity * Game.Mouse.PositionDelta.X / 16200.0f;
				UserCommand.Pitch		-=	2 * MathUtil.Pi * Config.Sensitivity * Game.Mouse.PositionDelta.Y / 16200.0f * ( Config.InvertMouse ? -1 : 1 );
				UserCommand.Roll		=	0;
			}


			var cmdBytes = UserCommand.GetBytes( UserCommand );

			gameWorld.RecordUserCommand( sentCommandID, gameTime.ElapsedSec, cmdBytes );
			gameWorld.PlayerCommand( this.Guid, cmdBytes, 0 );

			if (!ProcessSnapshot()) {
				gameWorld.SimulateWorld( gameTime.ElapsedSec );
			}

			gameWorld.PresentWorld( gameTime.ElapsedSec );

			return cmdBytes;
		}



		/// <summary>
		/// Process snapshot, replay world.
		/// </summary>
		/// <returns></returns>
		bool ProcessSnapshot ()
		{
			if (latestSnapshot!=null) {

				//	read snapshot :
				using ( var ms = new MemoryStream(latestSnapshot) ) {
					using ( var reader = new BinaryReader(ms) ) {
						gameWorld.Read( reader, ackCommandID );
					}
				}

				//	replay world with non-acked commands :
				gameWorld.ReplayWorld( ackCommandID );


				foreach ( var ent in gameWorld.entities ) {
					ent.Value.PositionError = ent.Value.PositionError - ent.Value.Position;
				}

				latestSnapshot	=	null;

				return true;

			} else {

				//foreach ( var ent in gameWorld.entities ) {
				//	var len = ent.Value.PositionError.Length();

				//	if (len<0.0001f) {
				//		continue;
				//	}

				//	len = Math.Max( 0, len - 0.1f );

				//	ent.Value.PositionError = ent.Value.PositionError.Normalized() * len;

				//	//ent.Value.PositionError = ent.Value.PositionError * 0.9f;
				//}

				return false;
			}
		}



		/// <summary>
		/// Feed server snapshot to client.
		/// Called when fresh snapshot arrived.
		/// </summary>
		/// <param name="snapshot"></param>
		public override void FeedSnapshot ( byte[] snapshot, uint ackCommandID )
		{
			//Log.Warning("Ack cmd : {0}", ackCommandID );
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
