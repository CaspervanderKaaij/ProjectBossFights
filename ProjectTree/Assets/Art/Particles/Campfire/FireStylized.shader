// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FireStylized"
{
	Properties
	{
		_NoiseTexture("Noise Texture", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.23
		_NoiseSpeed1("NoiseSpeed1", Vector) = (0.06,-0.03,0,0)
		_BorderIntensity("Border Intensity", Float) = 5
		_NoiseSpeed2("NoiseSpeed2", Vector) = (-0.09,0.06,0,0)
		_BorderTreshold("Border Treshold", Float) = 1.96
		_BorderSmoothness("Border Smoothness", Float) = 5.38
		[HDR]_Color("Color", Color) = (0,0,0,0)
		_Intensity("Intensity", Float) = 1
		_NoiseTiling1("NoiseTiling1", Vector) = (1,1,0,0)
		_NoiseTiling2("NoiseTiling2", Vector) = (1,1,0,0)
		_UberTiling("Uber Tiling", Vector) = (0,0,0,0)
		_Rotation("Rotation", Float) = 1
		_NoiseTimescale("Noise Timescale", Float) = 1
		_Mask("Mask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform sampler2D _NoiseTexture;
		uniform float _NoiseTimescale;
		uniform float2 _NoiseSpeed1;
		uniform float2 _NoiseTiling1;
		uniform float2 _UberTiling;
		uniform float2 _NoiseSpeed2;
		uniform float2 _NoiseTiling2;
		uniform float _Intensity;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform float _BorderSmoothness;
		uniform float _BorderTreshold;
		uniform float _Rotation;
		uniform float _BorderIntensity;
		uniform float _Cutoff = 0.23;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = _Color.rgb;
			o.Alpha = 1;
			float mulTime46 = _Time.y * _NoiseTimescale;
			float2 uv_TexCoord5 = i.uv_texcoord * ( _NoiseTiling1 + _UberTiling );
			float2 panner6 = ( mulTime46 * _NoiseSpeed1 + uv_TexCoord5);
			float2 uv_TexCoord4 = i.uv_texcoord * ( _NoiseTiling2 + _UberTiling );
			float2 panner7 = ( mulTime46 * _NoiseSpeed2 + uv_TexCoord4);
			float4 temp_output_10_0 = ( tex2D( _NoiseTexture, panner6, float2( 0,0 ), float2( 0,0 ) ) * tex2D( _NoiseTexture, panner7, float2( 0,0 ), float2( 0,0 ) ) * _Intensity );
			float2 uv0_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 temp_cast_1 = (_BorderSmoothness).xxxx;
			float4 temp_cast_2 = (_BorderTreshold).xxxx;
			float cos43 = cos( _Rotation );
			float sin43 = sin( _Rotation );
			float2 rotator43 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos43 , -sin43 , sin43 , cos43 )) + float2( 0.5,0.5 );
			float4 smoothstepResult23 = smoothstep( temp_cast_1 , temp_cast_2 , ( ( temp_output_10_0 + float4( rotator43, 0.0 , 0.0 ) ) * float4( rotator43, 0.0 , 0.0 ) * _BorderIntensity ));
			float4 smoothstepResult49 = smoothstep( float4( 0,0,0,0 ) , tex2D( _Mask, uv0_Mask ) , smoothstepResult23);
			clip( ( temp_output_10_0 * ( 1.0 - smoothstepResult49 ) ).r - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17200
0;73;1924;928;900.5388;-47.96152;1;True;False
Node;AmplifyShaderEditor.Vector2Node;15;-2335.782,504.6346;Inherit;False;Property;_NoiseTiling2;NoiseTiling2;10;0;Create;True;0;0;False;0;1,1;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;14;-2549.833,-49.84521;Inherit;False;Property;_NoiseTiling1;NoiseTiling1;9;0;Create;True;0;0;False;0;1,1;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;40;-2546.549,262.0938;Inherit;False;Property;_UberTiling;Uber Tiling;11;0;Create;True;0;0;False;0;0,0;0.1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-2322.577,304.103;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;38;-2341.965,137.9067;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1952.004,198.5616;Inherit;False;Property;_NoiseTimescale;Noise Timescale;14;0;Create;True;0;0;False;0;1;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-2080.884,15.34726;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-2076.284,423.0473;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;8;-1910.798,-114.1185;Inherit;False;Property;_NoiseSpeed1;NoiseSpeed1;2;0;Create;True;0;0;False;0;0.06,-0.03;0.06,-0.59;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;9;-1889.798,549.8816;Inherit;False;Property;_NoiseSpeed2;NoiseSpeed2;4;0;Create;True;0;0;False;0;-0.09,0.06;-0.09,-0.27;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;46;-1825.04,288.2884;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-2200.043,185.4485;Inherit;True;Property;_NoiseTexture;Noise Texture;0;0;Create;True;0;0;False;0;e28dc97a9541e3642a48c0e3886688c5;35d45d7ac35b17c4f97c2d040fb81e38;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;6;-1737.798,47.88153;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;7;-1718.798,375.8816;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1603.645,960.3698;Inherit;False;Property;_Rotation;Rotation;13;0;Create;True;0;0;False;0;1;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1473.318,-1.811445;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1454.358,412.3083;Inherit;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;13;-1108.202,566.8331;Inherit;False;Property;_Intensity;Intensity;8;0;Create;True;0;0;False;0;1;5.31;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-1454.251,695.7193;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-906.0857,143.903;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RotatorNode;43;-1434.71,858.4891;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;11;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-794.0872,474.588;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-765.8065,711.0897;Inherit;False;Property;_BorderIntensity;Border Intensity;3;0;Create;True;0;0;False;0;5;10.64;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;-412.4192,1200.725;Inherit;False;0;48;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-551.5155,474.3843;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-587.4848,827.6443;Inherit;False;Property;_BorderTreshold;Border Treshold;5;0;Create;True;0;0;False;0;1.96;1.96;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-578.491,969.098;Inherit;False;Property;_BorderSmoothness;Border Smoothness;6;0;Create;True;0;0;False;0;5.38;7.65;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;23;-173.2141,660.4296;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;48;-155.419,1149.325;Inherit;True;Property;_Mask;Mask;15;0;Create;True;0;0;False;0;-1;None;eb212d40e6ed388439c2adc90de97b6d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;49;160.8383,929.2755;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;37;82.17908,644.7697;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;42;-1121.149,956.8508;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;12;-277.5894,-79.86963;Inherit;False;Property;_Color;Color;7;1;[HDR];Create;True;0;0;False;0;0,0,0,0;7.377211,0.7724828,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;26;-311.2213,997.7671;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;41;-946.368,866.4176;Inherit;False;Property;_BorderYInversion;BorderYInversion;12;0;Create;True;0;0;False;0;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-280.9549,225.8536;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;ASEMaterialInspector;0;0;Standard;FireStylized;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.23;True;False;0;True;TransparentCutout;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;39;0;15;0
WireConnection;39;1;40;0
WireConnection;38;0;14;0
WireConnection;38;1;40;0
WireConnection;5;0;38;0
WireConnection;4;0;39;0
WireConnection;46;0;45;0
WireConnection;6;0;5;0
WireConnection;6;2;8;0
WireConnection;6;1;46;0
WireConnection;7;0;4;0
WireConnection;7;2;9;0
WireConnection;7;1;46;0
WireConnection;1;0;2;0
WireConnection;1;1;6;0
WireConnection;3;0;2;0
WireConnection;3;1;7;0
WireConnection;10;0;1;0
WireConnection;10;1;3;0
WireConnection;10;2;13;0
WireConnection;43;0;16;0
WireConnection;43;2;44;0
WireConnection;20;0;10;0
WireConnection;20;1;43;0
WireConnection;21;0;20;0
WireConnection;21;1;43;0
WireConnection;21;2;22;0
WireConnection;23;0;21;0
WireConnection;23;1;25;0
WireConnection;23;2;24;0
WireConnection;48;1;47;0
WireConnection;49;0;23;0
WireConnection;49;2;48;0
WireConnection;37;0;49;0
WireConnection;42;0;16;2
WireConnection;26;0;24;0
WireConnection;26;1;25;0
WireConnection;41;0;16;2
WireConnection;41;1;42;0
WireConnection;29;0;10;0
WireConnection;29;1;37;0
WireConnection;0;2;12;0
WireConnection;0;10;29;0
ASEEND*/
//CHKSM=83C5718C3D785F2AF45845DD49D731B56E4EC4C9