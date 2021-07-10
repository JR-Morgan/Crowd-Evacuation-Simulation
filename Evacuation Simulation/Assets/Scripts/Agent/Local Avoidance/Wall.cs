using UnityEngine;

namespace PedestrianSimulation.Agent.LocalAvoidance
{
    // This class has been adapted from https://github.com/fawwazbmn/SocialForceModel C++ SFM
    // Licence: BSD 3-Clause License https://github.com/fawwazbmn/SocialForceModel/blob/master/LICENSE
    public class Wall
    {
        private readonly struct Line
        {
            public readonly Vector3 start, end;
            public Line(Vector3 start, Vector3 end)
            {
                this.start = start;
                this.end = end;
            }
        };

        private readonly Line wall;

        public Wall()
            :this(default, default)
        { }

        public Wall(Vector3 start, Vector3 end)
        {
            wall = new Line(start, end);
        }

        public Vector3 StartPoint => wall.start;
        public Vector3 EndPoint => wall.end;

        public Vector3 GetNearestPoint(Vector3 position_i)
        {
            // Create Vector Relative to Wall's 'start'
            Vector3 relativeEnd = wall.end - wall.start;    // Vector from wall's 'start' to 'end'
            Vector3 relativePos = position_i - wall.start;  // Vector from wall's 'start' to agent i 'position'

            // Scale Both Vectors by the Length of the Wall
            Vector3 relativeEndScale = Vector3.Normalize(relativeEnd);

            Vector3 relativePosScale = relativePos * (1.0F / relativeEnd.magnitude);

            // Compute Dot Product of Scaled Vectors
            float dotProduct = Vector3.Dot(relativeEndScale, relativePosScale);
            
            Vector3 nearestPoint = Vector3.Lerp(wall.start,wall.end, dotProduct);
            
            return nearestPoint;
        }


    }
}
