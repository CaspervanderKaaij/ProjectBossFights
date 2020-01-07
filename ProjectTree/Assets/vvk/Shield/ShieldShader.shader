// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ShieldShader"
{
	Properties
	{
		[HDR]_BaseColor("Base Color", Color) = (0,0,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_TextureSample4("Texture Sample4", 2D) = "white" {}
		_Vector0("Vector 0", Vector) = (0,0,0,0)
		_DissolvePogress("DissolvePogress", Float) = 1
		_Vector05("Vector 0=5", Vector) = (0,0,0,0)
		[HDR]_FresnelColor("Fresnel Color", Color) = (0,0,0,0)
		_FresnelPower("Fresnel Power", Float) = 0
		_VertexStrength("VertexStrength", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma surface surf Standard alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _VertexStrength;
		uniform sampler2D _TextureSample0;
		uniform float2 _Vector0;
		uniform float4 _TextureSample0_ST;
		uniform float4 _BaseColor;
		uniform sampler2D _TextureSample4;
		uniform float2 _Vector05;
		uniform float4 _TextureSample4_ST;
		uniform float4 _FresnelColor;
		uniform float _FresnelPower;
		uniform float _DissolvePogress;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 ase_vertexNormal = v.normal.xyz;
			float2 uv0_TextureSample0 = v.texcoord.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float2 panner4 = ( 1.0 * _Time.y * _Vector0 + uv0_TextureSample0);
			v.normal = ( float4( ase_vertex3Pos , 0.0 ) + ( float4( ase_vertexNormal , 0.0 ) * _VertexStrength * tex2Dlod( _TextureSample0, float4( panner4, 0, 0.0) ) ) ).rgb;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv0_TextureSample4 = i.uv_texcoord * _TextureSample4_ST.xy + _TextureSample4_ST.zw;
			float2 panner21 = ( 1.0 * _Time.y * _Vector05 + uv0_TextureSample4);
			float4 tex2DNode22 = tex2D( _TextureSample4, panner21 );
			float4 temp_output_23_0 = ( _BaseColor * tex2DNode22 );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV6 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode6 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV6, _FresnelPower ) );
			o.Emission = ( temp_output_23_0 + ( _FresnelColor * fresnelNode6 ) ).rgb;
			float4 temp_cast_1 = (_DissolvePogress).xxxx;
			float4 clampResult18 = clamp( ( fresnelNode6 + temp_output_23_0 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
			o.Alpha = ( step( tex2DNode22 , temp_cast_1 ) * clampResult18 ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
849;73;674;651;433.6708;95.06815;1;True;False
Node;AmplifyShaderEditor.Vector2Node;19;-1159.027,30.65388;Inherit;False;Property;_Vector05;Vector 0=5;5;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;20;-1274.027,-152.3462;Inherit;False;0;22;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;21;-957.027,-57.34612;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1014.362,-536.7256;Inherit;False;0;3;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;5;-935.3621,-353.7255;Inherit;False;Property;_Vector0;Vector 0;3;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;8;-767.2598,446.1654;Inherit;False;Property;_FresnelPower;Fresnel Power;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-375.411,-215.3;Inherit;False;Property;_BaseColor;Base Color;0;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;22;-739.027,-125.3462;Inherit;True;Property;_TextureSample4;Texture Sample4;2;0;Create;True;0;0;False;0;-1;61c0b9c0523734e0e91bc6043c72a490;61c0b9c0523734e0e91bc6043c72a490;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;4;-733.3621,-441.7255;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FresnelNode;6;-531.2598,443.1654;Inherit;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-169.2373,-25.29163;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1075.805,274.0439;Inherit;False;Property;_DissolvePogress;DissolvePogress;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-143.2373,265.7084;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;12;-63.81689,-447.7983;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;7;-594.4598,202.2654;Inherit;False;Property;_FresnelColor;Fresnel Color;6;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-54.81689,-253.7983;Inherit;False;Property;_VertexStrength;VertexStrength;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-515.3623,-509.7256;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;-1;61c0b9c0523734e0e91bc6043c72a490;61c0b9c0523734e0e91bc6043c72a490;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;15;190.1831,-464.7983;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;169.1831,-238.7983;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-335.3804,231.6646;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;18;-28.11725,380.591;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;27;-822.615,134.239;Inherit;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;422.1831,-252.7983;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-30.2373,85.70837;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-4.079483,201.0105;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;148,4;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;ShieldShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;21;0;20;0
WireConnection;21;2;19;0
WireConnection;22;1;21;0
WireConnection;4;0;2;0
WireConnection;4;2;5;0
WireConnection;6;3;8;0
WireConnection;23;0;1;0
WireConnection;23;1;22;0
WireConnection;24;0;6;0
WireConnection;24;1;23;0
WireConnection;3;1;4;0
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;13;2;3;0
WireConnection;26;0;7;0
WireConnection;26;1;6;0
WireConnection;18;0;24;0
WireConnection;27;0;22;0
WireConnection;27;1;28;0
WireConnection;17;0;15;0
WireConnection;17;1;13;0
WireConnection;25;0;23;0
WireConnection;25;1;26;0
WireConnection;29;0;27;0
WireConnection;29;1;18;0
WireConnection;0;2;25;0
WireConnection;0;9;29;0
WireConnection;0;12;17;0
ASEEND*/
//CHKSM=7BFAE199003A20AB08609BA149561A6AA677A524