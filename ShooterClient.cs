﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fusion;
using Fusion.Core;
using Fusion.Core.Mathematics;
using Fusion.Engine.Common;
using Fusion.Engine.Input;
using Fusion.Engine.Client;
using Fusion.Engine.Server;
using Fusion.Engine.Graphics;
using ShooterDemo.Entities;


namespace ShooterDemo {
	class ShooterClient : Fusion.Engine.Client.GameClient {


		GameWorld	gameWorld;


		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="engine"></param>
		public ShooterClient ( Game game )
			: base( game )
		{
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
			Log.Message( "SERVER INFO : {0}", serverInfo );

			return new ShooterLoader( this, serverInfo );
		}



		/// <summary>
		/// Called when loader finished loading.
		/// This method lets client to complete loading process in main thread.
		/// </summary>
		/// <param name="loader"></param>
		public override void FinalizeLoad ( GameLoader loader )
		{
			gameWorld	= ((ShooterLoader)loader).World;

			var rw	=	Game.RenderSystem.RenderWorld;
			rw.ClearWorld();

			rw.Instances.Clear();

			foreach ( var inst in gameWorld.StaticMeshInstances ) {
				rw.Instances.Add( inst );
			}

			rw.Camera.SetupCameraFov( new Vector3(10,10,10), Vector3.Zero, Vector3.Up, MathUtil.Rad(90), 0.125f, 1024f, 1, 0, 1 );

			rw.HdrSettings.BloomAmount	= 0.2f;
			rw.HdrSettings.DirtAmount	= 0.0f;

			rw.SkySettings.SunPosition			=	Vector3.One;
			rw.SkySettings.SunLightIntensity	=	50;

			rw.LightSet.DirectLight.Direction	=	rw.SkySettings.SunLightDirection;
			rw.LightSet.DirectLight.Intensity	=	rw.SkySettings.SunLightColor;
			rw.LightSet.EnvLights.Add( new EnvLight( new Vector3(0,4,0), 1, 500 ) );

			rw.RenderRadiance();

			Game.GetModule<ShooterInterface>().ShowMenu = false;
		}



		/// <summary>
		///	Called when client disconnected, dropped, kicked or timeouted.
		///	Client must purge all level-associated content.
		///	Reason???
		/// </summary>
		public override void UnloadContent ()
		{
			gameWorld.KillAll();
			gameWorld	=	null;
			
			Game.RenderSystem.RenderWorld.ClearWorld();

			Content.Unload();
			Game.GetModule<ShooterInterface>().ShowMenu = true;
		}



		/// <summary>
		/// Runs one step of client-side simulation and renders world state.
		/// Do not close the stream.
		/// </summary>
		/// <param name="gameTime"></param>
		public override byte[] Update ( GameTime gameTime )
		{
			var userCmd = new UserCommand();
			userCmd.CtrlFlags	=	UserCtrlFlags.None;
			userCmd.Yaw			=	0;
			userCmd.Pitch		=	0;
			userCmd.Roll		=	0;

			if (Game.Keyboard.IsKeyDown(Keys.W)) {
				userCmd.CtrlFlags |= UserCtrlFlags.Forward;
			}
			if (Game.Keyboard.IsKeyDown(Keys.S)) {
				userCmd.CtrlFlags |= UserCtrlFlags.Backward;
			}


			var player = (Player)null;//GetPlayer();
			var aspect = Game.RenderSystem.DisplayBounds.Width / (float)Game.RenderSystem.DisplayBounds.Height;

			if (player!=null) {
				Game.RenderSystem.RenderWorld.Camera.SetupCameraFov( Matrix.Invert(player.World), MathUtil.Rad(90), 0.125f, 5000, 1, 0, aspect );
			} else {
				Game.RenderSystem.RenderWorld.Camera.SetupCameraFov( new Vector3(10,0,10), Vector3.Zero, Vector3.Up, MathUtil.Rad(90), 0.125f, 1024f, 1, 0, aspect );
			}



			return UserCommand.GetBytes( userCmd );
		}



		/// <summary>
		/// Feed server snapshot to client.
		/// Called when fresh snapshot arrived.
		/// </summary>
		/// <param name="snapshot"></param>
		public override void FeedSnapshot ( byte[] snapshot, bool initial )
		{
			Snapshot.ReadSnapshot( snapshot, gameWorld.Entities );
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
	}
}
