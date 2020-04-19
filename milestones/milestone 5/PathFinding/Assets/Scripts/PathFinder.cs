using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder
{
    public static TilePath DiscoverPath(Tilemap map, Vector3Int start, Vector3Int end)
    {
        //you will return this path to the user.  It should be the shortest path between
        //the start and end vertices 
        TilePath discoveredPath = new TilePath();

        //TileFactory is how you get information on tiles that exist at a particular vector's
        //coordinates
        TileFactory tileFactory = TileFactory.GetInstance();

        //This is the priority queue of paths that you will use in your implementation of
        //Dijkstra's algorithm
        PriortyQueue<TilePath> pathQueue = new PriortyQueue<TilePath>();

        //You can slightly speed up your algorithm by remembering previously visited tiles.
        //This isn't strictly necessary.
        Dictionary<Vector3Int, int> discoveredTiles = new Dictionary<Vector3Int, int>();

        //quick sanity check
        if (map == null || start == null || end == null)
        {
            return discoveredPath;
        }

        //This is how you get tile information for a particular map location
        //This gets the Unity tile, which contains a coordinate (.Position)
        var startingMapLocation = map.GetTile(start);
        //var endingMapLocation = map.GetTile(end);

        //And this converts the Unity tile into an object model that tracks the
        //cost to visit the tile.
        var startingTile = tileFactory.GetTile(startingMapLocation.name);
        startingTile.Position = start;

        //var endingTile = tileFactory.GetTile(endingMapLocation.name);
        //endingTile.Position = end;

        //Any discovered path must start at the origin!
        discoveredPath.AddTileToPath(startingTile);

        //This adds the starting tile to the PQ and we start off from there...
        pathQueue.Enqueue(discoveredPath);
        bool found = false;
        while (found == false && pathQueue.IsEmpty() == false)
        {
            //TODO: Implement Dijkstra's algorithm!
            TilePath current_path = new TilePath(pathQueue.Dequeue());

            // Step 1, we found the spot
            Vector3Int position = current_path.GetMostRecentTile().Position; // TO-DO: Get most recent tile's location, assign to variable

            if (position == end) // If currentLocation == End location
            {
                discoveredPath = current_path;
                break;
            }

            // Step 2, we didn't find the spot, Dijksta's algorithm part 2.

            // Do this 4 times, once for each of these locations:
            // where O is our current path, and X is the next path we go to
            //  _X_ ___ ___ ___
            //  _O_ _O_ _OX XO_
            //  ___ _X_ ___ ___

            // Get location of X

            Vector3Int next_down = position;
            Vector3Int next_up = position;
            Vector3Int next_right = position;
            Vector3Int next_left = position;
            next_down.y -= 1;
            next_up.y += 1;
            next_right.x += 1;
            next_left.x -= 1;

            Vector3Int[] next_poses = new Vector3Int[4];
            next_poses[0] = next_down;
            next_poses[1] = next_up;
            next_poses[2] = next_right;
            next_poses[3] = next_left;

            for (int i = 0; i < 4; i++)
            {
                // Get the tile and set to position of X
                var next_position = map.GetTile(next_poses[i]);
                if (next_position != null)
                {
                    // Get the tile
                    var new_tile = tileFactory.GetTile(next_position.name);
                    new_tile.Position = next_poses[i];

                    // Now we make a brand new path based on our current one.(bullet 1)
                    TilePath new_path = new TilePath(current_path); // Done

                    // Add new_tile to new_path
                    new_path.AddTileToPath(new_tile); // todo

                    // does it contain end tile?
                    if (next_poses[i] == end)
                    {
                        found = true;
                        discoveredPath = new_path;
                        break;
                    }
                    else
                    {
                        pathQueue.Enqueue(new_path);
                    }
                }
            }

            //pathQueue.Dequeue();


            //This line ensures that we don't get an infinite loop in Unity.
            //You will need to remove it in order for your pathfinding algorithm to work.
            //found = true;
        }
        
        return discoveredPath;
    }
}
