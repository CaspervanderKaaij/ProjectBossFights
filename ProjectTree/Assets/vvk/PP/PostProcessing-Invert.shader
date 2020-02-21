// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "InvertYeetWowman"
{
	Properties
	{
		_MainTex ( "Screen", 2D ) = "black" {}
		_InvertAlpha("Invert Alpha", Range( 0 , 1)) = 0
		_InvertStrength("InvertStrength", Float) = -0.1

	}

	SubShader
	{
		LOD 0

		
		
		ZTest Always
		Cull Off
		ZWrite Off

		
		Pass
		{ 
			CGPROGRAM 

			

			#pragma vertex vert_img_custom 
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			

			struct appdata_img_custom
			{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				
			};

			struct v2f_img_custom
			{
				float4 pos : SV_POSITION;
				half2 uv   : TEXCOORD0;
				half2 stereoUV : TEXCOORD2;
		#if UNITY_UV_STARTS_AT_TOP
				half4 uv2 : TEXCOORD1;
				half4 stereoUV2 : TEXCOORD3;
		#endif
				
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform float _InvertStrength;
			uniform float _InvertAlpha;


			v2f_img_custom vert_img_custom ( appdata_img_custom v  )
			{
				v2f_img_custom o;
				
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = float4( v.texcoord.xy, 1, 1 );

				#if UNITY_UV_STARTS_AT_TOP
					o.uv2 = float4( v.texcoord.xy, 1, 1 );
					o.stereoUV2 = UnityStereoScreenSpaceUVAdjust ( o.uv2, _MainTex_ST );

					if ( _MainTex_TexelSize.y < 0.0 )
						o.uv.y = 1.0 - o.uv.y;
				#endif
				o.stereoUV = UnityStereoScreenSpaceUVAdjust ( o.uv, _MainTex_ST );
				return o;
			}

			half4 frag ( v2f_img_custom i ) : SV_Target
			{
				#ifdef UNITY_UV_STARTS_AT_TOP
					half2 uv = i.uv2;
					half2 stereoUV = i.stereoUV2;
				#else
					half2 uv = i.uv;
					half2 stereoUV = i.stereoUV;
				#endif	
				
				half4 finalColor;

				// ase common template code
				float2 uv09 = i.uv.xy * float2( 1,1 ) + float2( 0,0 );
				float4 tex2DNode13 = tex2D( _MainTex, uv09 );
				float4 blendOpSrc16 = tex2DNode13;
				float4 blendOpDest16 = float4( 0,0,0,0 );
				float4 lerpBlendMode16 = lerp(blendOpDest16,( 1.0 - ( ( 1.0 - blendOpDest16) / max( blendOpSrc16, 0.00001) ) ),_InvertStrength);
				float4 lerpResult18 = lerp( tex2DNode13 , ( saturate( lerpBlendMode16 )) , _InvertAlpha);
				

				finalColor = lerpResult18;

				return finalColor;
			} 
			ENDCG 
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=17400
643;73;859;646;387.5097;-46.91429;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-658.2863,221.7757;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;1;-474.6995,109.6;Inherit;False;0;0;_MainTex;Shader;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;13;-243.3913,251.4997;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;-292.3893,625.6166;Inherit;False;Property;_InvertStrength;InvertStrength;1;0;Create;True;0;0;False;0;-0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;16;-74.19708,507.9172;Inherit;True;ColorBurn;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;-0.1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-20.46926,113.8464;Inherit;False;Property;_InvertAlpha;Invert Alpha;0;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;18;161.3862,215.8945;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;319.882,321.2145;Float;False;True;-1;2;ASEMaterialInspector;0;2;InvertYeetWowman;c71b220b631b6344493ea3cf87110c93;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;True;2;False;-1;False;False;True;2;False;-1;True;7;False;-1;False;True;0;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;0
WireConnection;13;0;1;0
WireConnection;13;1;9;0
WireConnection;16;0;13;0
WireConnection;16;2;17;0
WireConnection;18;0;13;0
WireConnection;18;1;16;0
WireConnection;18;2;19;0
WireConnection;0;0;18;0
ASEEND*/
//CHKSM=DE44F2EB099D38ADF4E5EBC2FB155FA99C46EB5E