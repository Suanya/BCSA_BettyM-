Shader "Unlit/LeinwandShaderUpdate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Pattern ("Pattern", 2D) = "white" {}

        _ColorA("Color A", Color) = ( 1,1,1,1 )
        _ColorB("Color B", Color) = ( 1,1,1,1 )

        _WaveAmp("WaveAmplitute", Range(0,0.2)) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           

            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            float4 _ColorA;
            float4 _ColorB;
            float _WaveAmp;

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normals : NORMAL;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;             
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD0; 
            };

            sampler2D _MainTex;
            sampler2D _Pattern;

            float4 _MainTex_ST; // optional          

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.worldPos =  mul( UNITY_MATRIX_M, v.vertex );
                o.vertex = UnityObjectToClipPos(v.vertex);
                v.vertex.y = GetWave(v.uv0) * _WaveAmp;
                o.uv = v.uv;
                return o;
            }

            float GetWave( float coord)
            {    
                float wave = cos((coord - _Time.y * 0.6) * TAU * 0.75) * 0.5 + 0.5; 
                wave *= coord; // 1 - coord;

                return wave;

                               
            }

            float GetAmplitude(float2 uv)
            {
                float2 uvsCentered = uv * 2 - 1;
                float radialDistance = length(uvsCentered);

                float ampli = cos( (radialDistance - _Time.y * 0.1 ) * TAU * 5) * 0.5 + 0.5;
          
                ampli *= radialDistance;
                return ampli;
            }

            // define own function 
            float InverseLerp(float a, float b, float v)
            {
                return(v-a)/(b-a);    
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float2 TopDownProjection = i.worldPos.xz;
                float4 ground = tex2D(_MainTex, TopDownProjection );
                float pattern = tex2D( _Pattern, i.uv );
      
                // return GetWave ( pattern );
                float4 gradient = lerp( _ColorA, _ColorB, i.uv.y );
                return gradient * GetWave ( pattern ) * GetAmplitude (i.uv);             
            }
            ENDCG
        }
    }
    }


/*
            float GetWave( float coord)
            {    
                float wave = cos((coord - _Time.y * 0.6) * TAU * 0.75) * 0.5 + 0.5; 
                wave *= coord; // 1 - coord;

                return wave;

                               
            }
            */
