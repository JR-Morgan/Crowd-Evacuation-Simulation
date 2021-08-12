Shader "Unlit/Test Shader"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
		_AgentSize("Agent Size", Range(0.0, 10)) = 1
		_Spread("Intensity", Range(0.00000001, 20)) = 1
		_Color0("Color 0", Color) = (0,0,0,1)
		_Color1("Color 1", Color) = (0,.9,.2,1)
		_Color2("Color 2", Color) = (.9,1,.3,1)
		_Color3("Color 3", Color) = (.9,.7,.1,1)
		_Color4("Color 4", Color) = (1,0,0,1)
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM

			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD0;
			};

			Buffer<float4> positionData;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}

			int numOfAgents; //Should never be greater than 1023
			//float4 positions[1023];

			float _AgentSize;
			float _Spread;

			#define NUM_COLORS 5
			float4 _Color0;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color4;

			//Returns value 0-1 based on a normal distribution
			float Intensity(const float distanceSquared)
			{
				static const float pi  = 3.1415926535897932;
				static const float e   = 2.7182818284590452;
			
				static const float d = -(sqrt(2) * sqrt(pi) * 0.5);
				
				return pow(e, d * distanceSquared);
			}

			float ToRange(const int i)
			{
				static const float rangeIncrement = 1.0 / (NUM_COLORS - 1.0);
				return rangeIncrement * i;
			}

			// Maps an intensity (0 - 1) to a color value between _Color0 and _Color4
			// intensity parameter may be above 1 and will be clip to _Color4
			float4 MapToColor(float intensity)
			{
				float4 colors[NUM_COLORS];
				int j = 0;
				colors[j++] = _Color0;
				colors[j++] = _Color1;
				colors[j++] = _Color2;
				colors[j++] = _Color3;
				colors[j++] = _Color4;

				float4 color = lerp(colors[0], colors[NUM_COLORS - 1], step(ToRange(NUM_COLORS - 1), intensity));

				float lowerRange = ToRange(0);

				for (int i = 1; i < NUM_COLORS; i++)
				{
					float upperRange = ToRange(i);

					//isInRange will be zero if the intensity does not fall within 
					float isInRange = step(intensity, upperRange) * step(lowerRange, intensity);

					float scaler = (intensity - lowerRange) / (upperRange - lowerRange);

					float4 colorDelta = (colors[i] - colors[i - 1]) * scaler;

					color += (colors[i - 1] + colorDelta) * isInRange;

					//Set current upperRange as the lowerRange for next itteration
					lowerRange = upperRange;

				}
				return color;
			}

			float4 frag(const v2f i) : SV_Target
			{
				float intensity = 0;

				for (int j = 0; j < numOfAgents; j++)
				{
					const float dv = positionData[j] - i.worldPos;
					//const float dis = sqrt(dot(dv,dv)) /_AgentSize;
					const float dis = distance(positionData[j], i.worldPos) /_AgentSize; 
					intensity += Intensity(dis * dis) / _Spread;
					
					//intensity += Intensity(distance(positionData[j], i.worldPos) / _AgentSize) / _Spread;
				}

				return MapToColor(intensity);
			}

			ENDCG
		}

	}
}