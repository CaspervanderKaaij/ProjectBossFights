// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Re-EntryFire"
{
	Properties
	{
		_Noise("Noise", 2D) = "white" {}
		_Skiddadle("Skiddadle", Float) = -0.1
		[HDR]_HighLight("HighLight", Color) = (0,0,0,0)
		[HDR]_Flame("Flame", Color) = (0,0,0,0)
		_Panning("Panning", Vector) = (0,0,0,0)
		_OpacityMultiplier("OpacityMultiplier", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Flame;
		uniform float4 _HighLight;
		uniform sampler2D _Noise;
		uniform float2 _Panning;
		uniform float4 _Noise_ST;
		uniform float _Skiddadle;
		uniform float _OpacityMultiplier;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv0_Noise = i.uv_texcoord * _Noise_ST.xy + _Noise_ST.zw;
			float2 panner12 = ( 1.0 * _Time.y * _Panning + uv0_Noise);
			float smoothstepResult7 = smoothstep( tex2D( _Noise, panner12 ).r , 1.0 , ( 1.0 - ( uv0_Noise.y - _Skiddadle ) ));
			float4 lerpResult9 = lerp( _Flame , _HighLight , smoothstepResult7);
			o.Emission = lerpResult9.rgb;
			o.Alpha = ( smoothstepResult7 * _OpacityMultiplier );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
911;73;594;651;501.3897;144.3363;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-1466.995,7.351591;Inherit;False;0;3;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;14;-1402.693,193.2995;Inherit;False;Property;_Panning;Panning;4;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;5;-704.6958,-34.40613;Inherit;False;Property;_Skiddadle;Skiddadle;1;0;Create;True;0;0;False;0;-0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;12;-1175.807,128.4747;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;4;-682.6958,65.59387;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;8;-502.6958,90.59387;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-940.6958,198.5939;Inherit;True;Property;_Noise;Noise;0;0;Create;True;0;0;False;0;-1;c60ce9d4c710434479c26cd5a98f7a9c;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;7;-491.6958,191.5939;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;10;-487.2811,-404.4875;Inherit;False;Property;_HighLight;HighLight;2;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-536.4722,-223.1013;Inherit;False;Property;_Flame;Flame;3;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;15;-218.3897,336.6637;Inherit;False;Property;_OpacityMultiplier;OpacityMultiplier;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;13;-981.3292,-19.40746;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;9;-289.6318,-62.08899;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-145.3897,216.6637;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Re-EntryFire;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Front;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;1;0
WireConnection;12;2;14;0
WireConnection;4;0;1;2
WireConnection;4;1;5;0
WireConnection;8;0;4;0
WireConnection;3;1;12;0
WireConnection;7;0;8;0
WireConnection;7;1;3;0
WireConnection;13;0;12;0
WireConnection;9;0;11;0
WireConnection;9;1;10;0
WireConnection;9;2;7;0
WireConnection;16;0;7;0
WireConnection;16;1;15;0
WireConnection;0;2;9;0
WireConnection;0;9;16;0
ASEEND*/
//CHKSM=BAE4673578BE03D9A3E67700A26017394867AFF1