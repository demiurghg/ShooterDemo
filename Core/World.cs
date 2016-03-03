using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fusion;
using Fusion.Core.Mathematics;
using Fusion.Engine.Common;
using Fusion.Core.Content;
using Fusion.Engine.Server;
using Fusion.Engine.Client;

namespace ShooterDemo.Core {

	/// <summary>
	/// World represents entire game state.
	/// </summary>
	public abstract class World {

		public readonly Guid UserGuid;

		public readonly Game Game;
		public readonly ContentManager Content;
		readonly bool serverSide;

		public delegate void EntityConstructor ( World world, Entity entity );
		public delegate void EntityEventHandler ( object sender, EntityEventArgs e );

		List<IEntityView> views = new List<IEntityView>();
		List<IEntityController> controllers = new List<IEntityController>();

		Dictionary<uint, Prefab> prefabs = new Dictionary<uint,Prefab>();

		public Dictionary<uint, Entity> entities;
		uint idCounter = 1;

		public event EntityEventHandler ReplicaSpawned;
		public event EntityEventHandler ReplicaKilled;


		/// <summary>
		/// We just received snapshot.
		/// Need to update client-side controllers.
		/// </summary>
		public bool snapshotDirty = false;


		class Prefab {
			public string Name;
			public EntityConstructor Construct;
		}


		/// <summary>
		/// Indicates that world is running on server side.
		/// </summary>
		public bool IsServerSide {
			get { return serverSide; }
		}



		/// <summary>
		/// Indicates that world is running on client side.
		/// </summary>
		public bool IsClientSide {
			get { return !serverSide; }
		}


		/// <summary>
		/// Gets server
		/// </summary>
		public GameServer GameServer {
			get { 
				if (!IsServerSide) {
					throw new System.Security.SecurityException("World is not server-side");
				}
				return Game.GameServer;
			}
		}


		/// <summary>
		/// Gets server
		/// </summary>
		public GameClient GameClient {
			get { 
				if (!IsClientSide) {
					throw new System.Security.SecurityException("World is nor client-side");
				}
				return Game.GameClient;
			}
		}


		/// <summary>
		/// Initializes server-side world.
		/// </summary>
		/// <param name="maxPlayers"></param>
		/// <param name="maxEntities"></param>
		public World ( GameServer server )
		{
			Log.Verbose("world: server");
			this.serverSide	=	true;
			this.Game		=	server.Game;
			this.UserGuid	=	new Guid();
			Content			=	server.Content;
			entities		=	new Dictionary<uint,Entity>();
		}



		/// <summary>
		/// Initializes client-side world.
		/// </summary>
		/// <param name="client"></param>
		public World ( GameClient client )
		{
			Log.Verbose("world: client");
			this.serverSide	=	false;
			this.Game		=	client.Game;
			this.UserGuid	=	client.Guid;
			Content			=	client.Content;
			entities		=	new Dictionary<uint,Entity>();
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="frmt"></param>
		/// <param name="args"></param>
		protected void LogTrace ( string frmt, params object[] args )
		{
			var s = string.Format( frmt, args );

			if (IsClientSide) Log.Verbose("cl: " + s );
			if (IsServerSide) Log.Verbose("sv: " + s );
		}



		/// <summary>
		/// Adds view.
		/// </summary>
		/// <param name="view"></param>
		public void AddView( IEntityView view )
		{
			if (IsServerSide) {
				return;
				//throw new InvalidOperationException("Can not add EntityView to server-side world");
			} 
			views.Add( view );
		}



		/// <summary>
		/// Adds controller.
		/// </summary>
		/// <param name="controller"></param>
		public void AddController ( IEntityController controller )
		{
			controllers.Add( controller );
		}



		/// <summary>
		/// Gets view by its type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetView<T>() where T: IEntityView 
		{
			return (T)views.FirstOrDefault( v => v is T );
		}



		/// <summary>
		/// Gets controller by its type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetController<T>() where T: IEntityController 
		{
			return (T)controllers.FirstOrDefault( c => c is T );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefabName"></param>
		/// <param name="constructAction"></param>
		public void AddPrefab ( string prefabName, EntityConstructor constructAction )
		{
			uint crc = Factory.GetPrefabID( prefabName );

			if (prefabs.ContainsKey(crc)) {
				throw new ArgumentException("Prefab '" + prefabName + "' cause hash collision with + '" + prefabs[crc].Name + "'");
			}

			prefabs.Add( crc, new Prefab(){ Name = prefabName, Construct = constructAction } );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefabId"></param>
		void ConstructEntity ( Entity entity )
		{
			prefabs[ entity.PrefabID ].Construct(this, entity);
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		void Destruct ( Entity entity )
		{
			foreach ( var controller in controllers ) {
				controller.Kill( entity.ID );
			}
			foreach ( var view in views ) {
				view.Kill( entity.ID );
			}
		}



		/// <summary>
		/// Simulates world.
		/// </summary>
		/// <param name="gameTime"></param>
		public virtual void SimulateWorld ( float deltaTime )
		{
			//
			//	Control entities :
			//
			//if (IsServerSide) {
				foreach ( var controller in controllers ) {
					controller.Update( deltaTime, snapshotDirty && IsClientSide );
				}
			//}
		}


		List<Vector3> clPos = new List<Vector3>();
		List<Vector3> visPos = new List<Vector3>();


		/// <summary>
		/// Updates visual and audial stuff
		/// </summary>
		/// <param name="gameTime"></param>
		public virtual void PresentWorld ( float deltaTime, float lerpFactor )
		{
			var dr = Game.RenderSystem.RenderWorld.Debug;

			//ForEachEntity( e => dr.Trace( e.Position, 0.25f, new Color(0,0,0,128) ) );
			ForEachEntity( e => dr.Trace( e.LerpPosition(lerpFactor), 0.05f, new Color(255,255,0,255) ) );


			if (IsClientSide) {
				foreach ( var view in views ) {
					view.Update( deltaTime, lerpFactor );
				}
			}
		}

		
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="parentId"></param>
		/// <param name="origin"></param>
		/// <param name="angles"></param>
		/// <returns></returns>
		public Entity Spawn ( string prefab, uint parentId, Vector3 origin, float angle )
		{	
			var a = new Angles();
			a.Yaw.Degrees = angle;
			return Spawn( prefab, parentId, origin, a );
		}



		/// <summary>
		/// When called on client-side returns null.
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="parent"></param>
		/// <param name="origin"></param>
		/// <param name="angles"></param>
		/// <returns></returns>
		public Entity Spawn ( string prefab, uint parentId, Vector3 origin, Angles angles )
		{
			//	due to server reconciliation
			//	never create entities on client-side:
			if (IsClientSide) {
				return null;
			}

			//	get ID :
			uint id = idCounter;

			idCounter++;

			if (idCounter==0) {
				//	this actually will never happen, about 103 day of intense playing.
				throw new InvalidOperationException("Too much entities were spawned");
			}


			uint prefabId = Factory.GetPrefabID( prefab );

			var entity = new Entity(id, prefabId, parentId, origin, angles);

			entities.Add( id, entity );

			ConstructEntity( entity );

			LogTrace("spawn: {0} - #{1}", prefab, id );

			return entity;
		}



		/// <summary>
		/// Check whether entity with id is dead.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool IsAlive ( uint id )
		{
			return entities.ContainsKey( id );
		}



		/// <summary>
		/// Gets entity with current id.
		/// If entity is dead -> exception...
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Entity GetEntity ( uint id )
		{
			return entities[ id ];
		}



		/// <summary>
		/// Gets entity with current id.
		/// If entity is dead -> exception...
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Entity GetEntityOrNull ( Func<Entity,bool> predicate )
		{
			return entities.FirstOrDefault( pair => predicate( pair.Value ) ).Value;
		}



		/// <summary>
		/// Gets entity with current id.
		/// If entity is dead -> exception...
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IEnumerable<Entity> GetEntities ( string prefabName )
		{
			uint prefabId = Factory.GetPrefabID( prefabName );

			return entities.Where( pair => pair.Value.PrefabID == prefabId ).Select( pair1 => pair1.Value );
		}


		/// <summary>
		/// Performs action on each entity.
		/// </summary>
		/// <param name="action"></param>
		public void ForEachEntity ( Action<Entity> action )
		{
			foreach ( var pair in entities ) {
				action( pair.Value );
			}
		}





		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public void Kill ( uint id )
		{
			if (id==0) {
				return;
			}

			LogTrace("kill: #{0}", id );

			Entity ent;

			if ( entities.TryGetValue(id, out ent)) {

				if (IsClientSide && ReplicaKilled!=null) {
					ReplicaKilled( this, new EntityEventArgs(ent) );
				}
				
				entities.Remove( id );
				Destruct( ent );

			} else {
				
				Log.Warning("Entity #{0} does not exist", id);
			
			}
		}




		/// <summary>
		/// Called when player enetered.
		/// </summary>
		/// <param name="guid"></param>
		public abstract void PlayerEntered ( Guid guid );

		/// <summary>
		/// Called when player left.
		/// </summary>
		/// <param name="guid"></param>
		public abstract void PlayerLeft ( Guid guid );

		/// <summary>
		/// Called when player left.
		/// </summary>
		/// <param name="guid"></param>
		public abstract void PlayerCommand ( Guid guid, byte[] command, float lag );

		/// <summary>
		/// This method called in main thread to complete non-thread safe operations.
		/// </summary>
		public abstract void FinalizeLoad (); 


		/// <summary>
		/// Returns server info.
		/// </summary>
		public abstract string ServerInfo (); 


		/// <summary>
		/// Called when client or server is 
		/// </summary>
		public abstract void Cleanup ();


		/// <summary>
		/// 
		/// </summary>
		public void PrintState ()
		{		
			var ents = entities.Select( pair => pair.Value ).OrderBy( e => e.ID ).ToArray();

			Log.Message("");
			Log.Message("---- {0} World state ---- ", IsServerSide ? "Server side" : "Client side" );

			foreach ( var ent in ents ) {
				
				var id		=	ent.ID;
				var parent	=	ent.ParentID;
				var prefab	=	prefabs[ ent.PrefabID ].Name;
				var guid	=	ent.UserGuid;

				Log.Message("{0:X8} {1:X8} {2} {3,-32}", id, parent, guid, prefab );
			}

			Log.Message("----------------" );
			Log.Message("");
		}


		/// <summary>
		/// Writes world state to stream writer.
		/// </summary>
		/// <param name="writer"></param>
		public virtual void Write ( BinaryWriter writer )
		{
			var entArray = entities.OrderBy( pair => pair.Key ).ToArray();

			writer.WriteFourCC("ENT0");

			writer.Write( entArray.Length );

			foreach ( var ent in entArray ) {
				writer.Write( ent.Key );
				ent.Value.Write( writer );
			}
		}



		/// <summary>
		/// Reads world state from stream reader.
		/// </summary>
		/// <param name="writer"></param>
		public virtual void Read ( BinaryReader reader, uint ackCmdID )
		{
			reader.ExpectFourCC("ENT0", "Bad snapshot");

			int length	=	reader.ReadInt32();
			var oldIDs	=	entities.Select( pair => pair.Key ).ToArray();
			var newIDs	=	new uint[length];


			for ( int i=0; i<length; i++ ) {

				uint id		=	reader.ReadUInt32();
				newIDs[i]	=	id;

				if ( entities.ContainsKey(id) ) {

					//	Entity with given ID exists.
					//	Just update internal state.
					entities[id].Read( reader );

				} else {
					
					//	Entity does not exist.
					//	Create new one.
					var ent = new Entity(id);

					ent.Read( reader );
					entities.Add( id, ent );

					ConstructEntity( ent );

					if (ReplicaSpawned!=null) {
						ReplicaSpawned( this, new EntityEventArgs(ent) );
					}
				}
			}

			//	Kill all stale entities :
			var staleIDs = oldIDs.Except( newIDs );

			foreach ( var id in staleIDs ) {
				Kill( id );
			}
		}
	}
}
