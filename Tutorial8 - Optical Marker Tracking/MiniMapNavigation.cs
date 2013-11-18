using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Generic;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Helpers;
using GoblinXNA.UI;
using GoblinXNA.UI.UI2D;


namespace Tutorial8___Optical_Marker_Tracking___PhoneLib
{
    class MiniMapNavigation
    {
        private Dictionary<int, Matrix> fires;
        private Matrix pos;
        private List<Vector3> fire_positions = new List<Vector3>();
        private readonly Vector3 offset = new Vector3(0, 0, 200);
        //private Matrix path;
        private float scale;

        public MiniMapNavigation(Dictionary<int, Matrix> fires)
        {
            this.fires = fires;
            //this.path = new Matrix();
            this.pos = new Matrix();
        }

        public void set_position(int key, Matrix matrix)
        {
            matrix.Translation = this.scale*((this.scale * fires[key].Translation + offset) + matrix.Translation);
            matrix.Translation = new Vector3(matrix.Translation.X, 0, matrix.Translation.Z);
            this.pos = matrix;
            //this.pos.Translation = this.scale*(fires[key].Translation - matrix.Translation);
        }

        public void remove_fire(int key)
        {
            this.fires.Remove(key);
        }
        /*
        public Vector3 get_path()
        {
            Vector3 temp;
            float distance = -1;
            foreach (int i in this.fires.Keys)
            {
                temp = this.fires[i] - this.pos;
                if (-1 == distance)
                {
                    this.path = temp;
                    distance = temp.Length();
                    continue;
                }
                if (temp.Length() < this.path.Length())
                {
                    this.path = temp;
                    distance = temp.Length();
                }
            }
            return this.path;
        }*/

        public void set_scale()
        {
            float largest = 0;
            float scale_factor = 0;
            foreach (int i in this.fires.Keys)
            {
                if (Math.Abs(fires[i].Translation.Z) > largest)
                {
                    largest = Math.Abs(fires[i].Translation.Z);
                    scale_factor = 400;
                }
                if (Math.Abs(fires[i].Translation.X) > largest)
                {
                    largest = Math.Abs(fires[i].Translation.X);
                    scale_factor = 400;
                }

            }
            this.scale = scale_factor / largest;
            this.scale = Math.Abs(this.scale);
        }

        public void init_positions()
        {
            foreach (int i in this.fires.Keys)
            {
                fire_positions.Add((this.scale * fires[i].Translation + offset));
            }
        }

        public List<Vector3> get_positions()
        {
            return fire_positions;
        }

        public float get_scale()
        {
            return this.scale;
        }

        public Matrix get_position()
        {
            return this.pos;
        }

    }
}