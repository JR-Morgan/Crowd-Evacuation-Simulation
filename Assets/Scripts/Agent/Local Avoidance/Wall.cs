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
            Vector3 relativeEnd, relativePos, relativeEndScal, relativePosScal;
            float dotProduct;
            Vector3 nearestPoint;

            // Create Vector Relative to Wall's 'start'
            relativeEnd = wall.end - wall.start;    // Vector from wall's 'start' to 'end'
            relativePos = position_i - wall.start;  // Vector from wall's 'start' to agent i 'position'

            // Scale Both Vectors by the Length of the Wall
            relativeEndScal = relativeEnd;
            relativeEndScal = relativeEndScal.normalized;

            relativePosScal = relativePos * (1.0F / relativeEnd.magnitude);

            // Compute Dot Product of Scaled Vectors
            dotProduct = Vector3.Dot(relativeEndScal, relativePosScal);

            if (dotProduct < 0.0)       // Position of Agent i located before wall's 'start'
                nearestPoint = wall.start;
            else if (dotProduct > 1.0)  // Position of Agent i located after wall's 'end'
                nearestPoint = wall.end;
            else                        // Position of Agent i located between wall's 'start' and 'end'
                nearestPoint = (relativeEnd * dotProduct) + wall.start;

            return nearestPoint;
        }


    }
}
