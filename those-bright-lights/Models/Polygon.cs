﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using SE_Praktikum.Extensions;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework.Graphics;


namespace SE_Praktikum.Models
{
    public class Polygon
    {

        private Vector2 _origin;
        private Vector2 _position;
        private List<Vector2> _vertices;
        private float _rotation;
        private float _layer;
        private Color _color;
        /// <summary>
        /// Simple polygon class
        /// it is able to handle to calculate whether two convex polygon intersect
        /// </summary>
        /// <param name="position">World coordinate of origin</param>
        /// <param name="origin">Zero point offset in body space</param>
        /// <param name="layer"></param>
        /// <param name="vertices">Corner points of convex polygon. please enter them clockwise and body coordinates</param>
        public Polygon(Vector2 position, Vector2 origin, float layer, List<Vector2> vertices, Color? color = null)
        {
            _vertices = vertices;
            _position = position;
            _origin = origin;
            _layer = layer;
            _color = color ?? Color.Yellow;
            Vertices2D = GetVector2InWorldSpace();
            Vertices3D = GetVector3InWorldSpace();
            VertexDrawingOrder = GetIndexMeshInts();
            DrawAbleVertices = GetDrawAbleVertices();
        }


        public float Rotation
        {
            
            get => _rotation;
            set
            {
                _rotation = value;
                Vertices2D = GetVector2InWorldSpace();
                Vertices3D = GetVector3InWorldSpace();
                VertexDrawingOrder = GetIndexMeshInts();
                DrawAbleVertices = GetDrawAbleVertices();
            }
        }

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                Vertices2D = GetVector2InWorldSpace();
                Vertices3D = GetVector3InWorldSpace();
                VertexDrawingOrder = GetIndexMeshInts();
                DrawAbleVertices = GetDrawAbleVertices();
            }
        }

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                DrawAbleVertices = GetDrawAbleVertices();
            }
        }

        public Vector2 Origin
        {
            
            get => _origin;
            set
            {
                _origin = value;
                Vertices2D = GetVector2InWorldSpace();
                Vertices3D = GetVector3InWorldSpace();
                VertexDrawingOrder = GetIndexMeshInts();
                DrawAbleVertices = GetDrawAbleVertices();
            }
        }
        
        /// <summary>
        /// accesses to vertices in body space
        /// </summary>
        public List<Vector2> Vertices
        {
            get => _vertices;
            set
            {
                _vertices = value;
                Vertices2D = GetVector2InWorldSpace();
                Vertices3D = GetVector3InWorldSpace();
                VertexDrawingOrder = GetIndexMeshInts();
                DrawAbleVertices = GetDrawAbleVertices();
            }
        }

        /// <summary>
        /// returns the vertices in world space
        /// </summary>
        public Vector2[] Vertices2D
        {
            get;
            private set;
        }

        /// <summary>
        /// returns the 3d vertices in world space
        /// </summary>
        public Vector3[] Vertices3D
        {
            get;
            private set;
        }

        public VertexPositionColor[] DrawAbleVertices
        {
            get;
            private set;
        }

        public int TriangleCount => VertexDrawingOrder.Count()/ 3;
        public int[] VertexDrawingOrder
        {
            get;
            private set;
        }

        public Vector2 Center => Position - Origin;

        public float Layer
        {
            get => _layer;
            set
            {
                _layer = value;
                Vertices3D = GetVector3InWorldSpace();
                VertexDrawingOrder = GetIndexMeshInts();
                DrawAbleVertices = GetDrawAbleVertices();
            }
        }

        #region Methods

        public bool Overlap(Polygon other)
        {
            for (var repetition = 0; repetition < 2; repetition++)
            {
                Polygon p1;
                Polygon p2;
                if (repetition == 0)
                {
                    p1 = this;
                    p2 = other;
                }
                else
                {
                    p1 = other;
                    p2 = this;
                }
                var normals = p1.GetNormals();

                foreach (var normal in normals)
                {
                    var minMax = p1.VectorProjection(normal);
                    var otherMinMax = p2.VectorProjection(normal);

                    if (!(otherMinMax[2] > minMax[1] && minMax[2] > otherMinMax[1]))
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Private Methods

        private List<Vector2> GetEdges()
        {
            var l = new List<Vector2>();
            for (var i = 0; i < _vertices.Count; i++)
            {
                //calculate next i modulo length of points to also get the edge from last to first point
                var nextI = i+1 % _vertices.Count;
                l.Add(new Vector2(_vertices[nextI].X - _vertices[i].X, _vertices[new Index()].Y - _vertices[i].Y));
            }

            return l;
        }

        private List<Vector2> GetNormals()
        {
            var edges = GetEdges();
            var normals = new List<Vector2>();
            for (var i = 0; i < edges.Count; i++)
            {
                normals.Add(new Vector2(-edges[i].Y, edges[i].X));
            }

            return normals;
        }

        private float[] VectorProjection(Vector2 normal)
        {
            var minMax = new float[2];
            //initialize minMax
            minMax[1] = float.PositiveInfinity;
            minMax[2] = float.NegativeInfinity;
            //dp == dotprodukt (Skalarprodukt)
            for (var j = 0; j < _vertices.Count; j++)
            {
                var dp = (normal.X * _vertices[j].X + normal.Y * _vertices[j].Y);
                //is dp smaller than current minimum
                if (dp < minMax[1])
                    minMax[1] = dp;
                //dp greater than current max?
                if (dp > minMax[2])
                    minMax[2] = dp;
            }
            return minMax;
        }

        private Vector2[] GetVector2InWorldSpace()
        {
            Vector2[] v = new Vector2[_vertices.Count];

            for (var index = 0; index < v.Count(); index++)
            {
                v[index] = Position + (-Origin + _vertices[index]).Rotate(Rotation);
            }

            return v;
        }

        private Vector3[] GetVector3InWorldSpace()
        {
            var v3 = new Vector3[_vertices.Count()];
            for (var index = 0; index < v3.Count(); index++)
            {
                v3[index] = new Vector3(Position + (-Origin + _vertices[index]).Rotate(Rotation), Layer);
            }
            return v3;
        }

        private int[] GetIndexMeshInts()
        {
            var diagonals = _vertices.Count - 3;
            var ind = new int[_vertices.Count + diagonals * 2];
            for(var i = 0; i < diagonals+1; i++)
            {
                var index = i * 3;
                ind[index] = 0;
                ind[index+1] = i+2;
                ind[index+2] = i+1;
            }
            return ind;
        }

        private VertexPositionColor[] GetDrawAbleVertices()
        {
            var n = _vertices.Count;
            var v = new VertexPositionColor[n];
            for (int i = 0; i < n; i++)
            {
                v[i].Position = Vertices3D[i];
                v[i].Color = Color;
            }
            return v;
        }
        

        #endregion
    }
}
