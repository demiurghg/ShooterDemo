using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fusion;
using Fusion.Engine.Client;
using Fusion.Engine.Common;
using Fusion.Engine.Server;
using Fusion.Core.Content;
using Fusion.Engine.Graphics;
using ShooterDemo.Entities;

namespace ShooterDemo {
	partial class ShooterServer : GameServer {

		GameWorld	gameWorld;


		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="engine"></param>
		public ShooterServer ( Game game )
			: base( game )
		{
		}



		/// <summary>
		/// 
		/// </summary>
		public override void Initialize ()
		{
		}



		/// <summary>
		/// Releases all resources used by the GameServer class.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose ( bool disposing )
		{
			if (disposing) {
				//	...
			}
			base.Dispose( disposing );
		}



		/// <summary>
		/// Method is invoked when server started.
		/// </summary>
		/// <param name="map"></param>
		public override void LoadContent ( string map )
		{
			gameWorld	=	new GameWorld( this, map );
		}



		/// <summary>
		/// Method is invoked when server shuts down.
		/// This method will be also called when server crashes.
		/// </summary>
		public override void UnloadContent ()
		{
			gameWorld	=	null;
			Content.Unload();
		}



		/// <summary>
		/// Runs one step of server-side world simulation.
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns>Snapshot bytes</returns>
		public override byte[] Update ( GameTime gameTime )
		{
			Thread.Sleep(1);

			gameWorld.Simulate( gameTime );
			gameWorld.Think( gameTime );

			//	write snapshot :
			return Snapshot.WriteSnapshot( gameWorld.Entities );
		}



		/// <summary>
		/// Feed client commands from particular client.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="clientId"></param>
		public override void FeedCommand ( Guid id, byte[] userCommand )
		{
			if (!userCommand.Any()) {
				return;
			}

			gameWorld.Command( id, userCommand );
		}



		/// <summary>
		/// Feed server notification from particular client.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="message"></param>
		public override void FeedNotification ( Guid id, string message )
		{
			Log.Message( "NOTIFICATION {0}: {1}", id, message );
		}



		/// <summary>
		/// Gets server information that required for client to load the game.
		/// This information usually contains map name and game type.
		/// This information is also used for discovery response.
		/// </summary>
		/// <returns></returns>
		public override string ServerInfo ()
		{
			return gameWorld.Info;
		}



		/// <summary>
		/// Notifies server that client connected.
		/// </summary>
		public override void ClientConnected ( Guid id, string userInfo )
		{
			NotifyClients( "CONNECTED: {0} - {1}", id, userInfo );
			Log.Message( "CONNECTED: {0} - {1}", id, userInfo );
		}


		public override void ClientActivated ( Guid guid )
		{
			gameWorld.PlayerEntered( guid );
		}


		public override void ClientDeactivated ( Guid guid )
		{
			gameWorld.PlayerLeft( guid );
		}


		/// <summary>
		/// Notifies server that client disconnected.
		/// </summary>
		public override void ClientDisconnected ( Guid id )
		{
			NotifyClients( "DISCONNECTED: {0}", id );
			Log.Message( "DISCONNECTED: {0}", id );
		}



		/// <summary>
		/// Approves client by id and user info.
		/// </summary>
		public override bool ApproveClient ( Guid id, string userInfo, out string reason )
		{
			Log.Message( "APPROVE: {0} {1}", id, userInfo );
			reason = "";
			return true;
		}
	}
}
