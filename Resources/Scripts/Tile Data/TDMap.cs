using UnityEngine;
using System.Collections.Generic;

public class TDMap
{
    protected class TDRoom
    {
        public int left;
        public int top;
        public int width;
        public int height;
        public bool isConnected;

        public int right
        {
            get { return left + width - 1; }
        }
        public int bottom
        {
            get { return top + height - 1; }
        }
        public int center_x
        {
            get { return left + width / 2; }
        }
        public int center_z
        {
            get { return top + height / 2; }
        }

        public bool CollidesWith(TDRoom other)
        {
            if (left > other.right - 1)
                return false;
            if (top > other.bottom - 1)
                return false;
            if (right < other.left + 1)
                return false;
            if (bottom < other.top + 1)
                return false;

            return true;
        }


    }

    int mapWidth;
    int mapHeight;

    int[,] map_data;

    // default to a 50x50 map size if nothing specified for constructor below
    public TDMap()
    {
		new TDMap(50, 50, 4, 16, 10, 10, 0.725f); // default it
    }

	public TDMap(int mapWidth, int mapHeight, int minRoomSize, int maxRoomSize, int totalRooms, int seed, float noiseFactor)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
		Vector2 shift = new Vector2(Random.Range(0, 32),seed);
		float zoom = Random.Range(0.095f, 0.1f);
        map_data = new int[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
				Vector2 pos = zoom * (new Vector2(x, z)) + shift;
				float perlin = Mathf.PerlinNoise(pos.x * noiseFactor, pos.y * noiseFactor);

				// Divy up tiles via noise level/height. This is tedious, can I clean it up?
				// Because seriously. Seriously. 
				if (perlin >= 0.0f && perlin <= 0.15f)
				{
					map_data[x, z] = TDTile.WATER;
				}
				else if (perlin > 0.15f && perlin < 0.2f)
				{
					map_data[x, z] = TDTile.SAND;
				}
				else if (perlin > 0.2f && perlin <= 0.7f)
				{
					if (perlin > 0.2f && perlin < 0.6f) {
						if (Random.Range(1, 128) > 125) {
							map_data[x, z] = TDTile.BUSH_BERRY;
						} 
						else {
							map_data[x, z] = TDTile.GRASS;
						}
					}
					else if (perlin > 0.6f && perlin < 0.605f) {
						map_data[x, z] = TDTile.BUSH;
					}
					else if (perlin > 0.62f && perlin < 0.63f) {
						map_data[x, z] = TDTile.TREE;
					}
					else {
						map_data[x, z] = TDTile.GRASS_THICK;
					}

				}
				else if (perlin > 0.7f && perlin <= 0.8f)
				{
					if (perlin > 0.7f && perlin < 0.72f) {
						map_data[x, z] = TDTile.TREE_PINE;
					}
					else if (perlin > 0.72f && perlin < 0.74f) {
						map_data[x, z] = TDTile.DIRT;
					}
					else {
						map_data[x, z] = TDTile.HILL;
					}

				}
				else
				{
					map_data[x, z] = TDTile.MOUNTAIN;
				}
            }
        }

		/*
		List<TDRoom> rooms;
		TDRoom r;
        rooms = new List<TDRoom>();

        int maxFails = 3;
        while (rooms.Count < totalRooms) // max room amount?
        { 
            int roomSizeX = Random.Range(minRoomSize, maxRoomSize);
            int roomSizeZ = Random.Range(minRoomSize, maxRoomSize);

            r = new TDRoom();
            r.left = Random.Range(0, mapWidth - roomSizeX);
            r.top = Random.Range(0, mapHeight - roomSizeZ);
            r.width = roomSizeX;
            r.height = roomSizeZ;

            // check for collision, so no overlaps
            if (!RoomCollides(r))
            {
                // Store the room for later
                rooms.Add(r);
            }
            else
            {
                maxFails--;
                if (maxFails <= 0)
                {
                    break;
                }
            }
        }
        // Now make the room visually
        foreach (TDRoom r2 in rooms)
        {
            MakeRoom(r2);
        }

        // now make connecting hallways
        for (int i = 0; i < rooms.Count; i++)
        {
            if (!rooms[i].isConnected)
            {
                int j = Random.Range(1, rooms.Count);

                MakeHallway(rooms[i], rooms[(i + j) % rooms.Count]);
            }
        }

        // now make additional walls around map
        MakeWalls();

		*/
        
    }

    public int GetTileAt ( int x, int z)
    {
        return map_data[x, z];
    }

	/*
    bool HasAdjacentFloor(int x, int z)
    {
    	// There could be a cleaner way to do this. I'll need to think it over.
    	
        // check left
        if (x > 0 && map_data[x - 1, z] == TDTile.SAND)
            return true;
        // check right
        if (x < mapWidth - 1 && map_data[x + 1, z] == TDTile.SAND)
            return true;
        // check top
        if (z > 0 && map_data[x, z - 1] == TDTile.SAND)
            return true;
        // check bottom
        if (z < mapHeight - 1 && map_data[x, z + 1] == TDTile.SAND)
            return true;

        // check top left / right
        if (x > 0 && z > 0 && map_data[x - 1, z - 1] == TDTile.SAND)
            return true;
        if (x < mapWidth - 1 && z > 0 && map_data[x + 1, z - 1] == TDTile.SAND)
            return true;

        //check bottom left/right
        if (x > 0 && z < mapHeight - 1 && map_data[x - 1, z + 1] == TDTile.SAND)
            return true;
        if (x < mapWidth - 1 && z < mapHeight - 1 && map_data[x + 1, z + 1] == TDTile.SAND)
            return true;


        return false;
    }


	bool RoomCollides(TDRoom r)
	{
		foreach(TDRoom r2 in rooms)
		{
			if (r.CollidesWith(r2))
			{
				return true;
			}
		}

		return false;
	}

	void MakeRoom(TDRoom r)
	{
		for (int x = 0; x < r.width; x++)
		{
			for (int z = 0; z < r.height; z++)
			{
				if (x == 0 || x == r.width - 1 || z == 0 || z == r.height - 1)
				{
					// walls
					map_data[r.left + x, r.top + z] = TDTile.DIRT;
				}
				else
				{
					// floor
					map_data[r.left + x, r.top + z] = TDTile.SAND;
				}
			}
		}
	}

	void MakeHallway(TDRoom r1, TDRoom r2)
	{
		int x = r1.center_x;
		int z = r1.center_z;

		while (x != r2.center_x)
		{
			map_data[x, z] = TDTile.SAND;
			x += x < r2.center_x ? 1 : -1;
		}

		while (z != r2.center_z)
		{
			map_data[x, z] = TDTile.SAND;
			z += z < r2.center_z ? 1 : -1;
		}

		r1.isConnected = true;
		r2.isConnected = true;
	}

	void MakeWalls()
	{
		for (int x = 0; x < mapWidth; x++)
		{
			for (int z = 0; z < mapHeight; z++)
			{
				// if our map data is equal to our default for the map (grass)
				if (map_data[x, z] == TDTile.GRASS && HasAdjacentFloor(x, z))
				{
					map_data[x, z] = TDTile.DIRT;
				}
			}
		}
	}
	*/
    
}
