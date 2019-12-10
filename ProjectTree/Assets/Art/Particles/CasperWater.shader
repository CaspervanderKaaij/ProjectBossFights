// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Remasu/Casper/CartoonWater"
{
	Properties
	{
		_EdgeDistance("Edge Distance", Float) = 1
		_BaseEdgeDistance("Base Edge Distance", Float) = 1
		_EdgePower("Edge Power", Range( 0 , 1)) = 0.5
		_BaseEdgePower("Base Edge Power", Range( 0 , 1)) = 0.5
		_TimeScale("TimeScale", Float) = 1
		_FoamNoiseScale("FoamNoiseScale", Float) = 4.34
		_FoamPan1("FoamPan1", Vector) = (0,0,0,0)
		_FoamPan2("FoamPan2", Vector) = (0,0,0,0)
		_StylizerMin("StylizerMin", Float) = 0
		_StylizerMax("StylizerMax", Float) = 1
		_BaseColor("Base Color", Color) = (0,0.5669932,1,0)
		_Foam("Foam", 2D) = "white" {}
		_FoamTimescale("Foam Timescale", Float) = 1
		_WaveOpacity("WaveOpacity", Float) = 1
		_WavesSmoothstep("WavesSmoothstep", Vector) = (0,0,0,0)
		_TEmp("TEmp", Vector) = (0,1,0,0)
		_WaterDirection("WaterDirection", Vector) = (0,0,0,0)
		_WaterDirectionScale("WaterDirectionScale", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform float4 _BaseColor;
		uniform float _StylizerMin;
		uniform float _StylizerMax;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _EdgeDistance;
		uniform float _EdgePower;
		uniform float _TimeScale;
		uniform float2 _FoamPan1;
		uniform float _FoamNoiseScale;
		uniform float2 _FoamPan2;
		uniform float _BaseEdgeDistance;
		uniform float _BaseEdgePower;
		uniform float2 _TEmp;
		uniform float2 _WavesSmoothstep;
		uniform sampler2D _Foam;
		uniform float _FoamTimescale;
		uniform float4 _Foam_ST;
		uniform float _WaterDirectionScale;
		uniform float2 _WaterDirection;
		uniform float _WaveOpacity;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_output_26_0 = _BaseColor;
			o.Albedo = temp_output_26_0.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth1 = abs( ( screenDepth1 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _EdgeDistance ) );
			float clampResult6 = clamp( ( ( 1.0 - distanceDepth1 ) * _EdgePower ) , 0.0 , 1.0 );
			float SecondaryEdge37 = clampResult6;
			float mulTime12 = _Time.y * _TimeScale;
			float2 panner10 = ( mulTime12 * _FoamPan1 + i.uv_texcoord);
			float simplePerlin2D7 = snoise( panner10*_FoamNoiseScale );
			float2 panner11 = ( mulTime12 * _FoamPan2 + i.uv_texcoord);
			float simplePerlin2D8 = snoise( panner11*_FoamNoiseScale );
			float clampResult18 = clamp( ( simplePerlin2D7 * simplePerlin2D8 ) , 0.0 , 1.0 );
			float clampResult20 = clamp( ( SecondaryEdge37 * clampResult18 ) , 0.0 , 1.0 );
			float screenDepth31 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth31 = abs( ( screenDepth31 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _BaseEdgeDistance ) );
			float clampResult33 = clamp( ( ( 1.0 - distanceDepth31 ) * _BaseEdgePower ) , 0.0 , 1.0 );
			float MainEdge36 = clampResult33;
			float smoothstepResult22 = smoothstep( _StylizerMin , _StylizerMax , ( saturate( clampResult20 ) + MainEdge36 ));
			float Edge41 = smoothstepResult22;
			float4 temp_cast_1 = (_WavesSmoothstep.x).xxxx;
			float4 temp_cast_2 = (_WavesSmoothstep.y).xxxx;
			float mulTime62 = _Time.y * _FoamTimescale;
			float4 appendResult67 = (float4(_FoamPan1.x , _FoamPan1.y , 0.0 , 0.0));
			float4 vel164 = appendResult67;
			float2 uv0_Foam = i.uv_texcoord * _Foam_ST.xy + _Foam_ST.zw;
			float mulTime89 = _Time.y * _WaterDirectionScale;
			float2 temp_output_87_0 = (uv0_Foam*1.0 + ( mulTime89 * _WaterDirection ));
			float2 panner60 = ( mulTime62 * vel164.xy + temp_output_87_0);
			float4 tex2DNode53 = tex2D( _Foam, panner60 );
			float4 appendResult68 = (float4(_FoamPan2.x , _FoamPan2.y , 0.0 , 0.0));
			float4 vel269 = appendResult68;
			float2 panner61 = ( mulTime62 * vel269.xy + temp_output_87_0);
			float cos85 = cos( 3.141593 );
			float sin85 = sin( 3.141593 );
			float2 rotator85 = mul( panner61 - float2( 0.5,0.5 ) , float2x2( cos85 , -sin85 , sin85 , cos85 )) + float2( 0.5,0.5 );
			float4 smoothstepResult55 = smoothstep( temp_cast_1 , temp_cast_2 , ( tex2DNode53 * ( tex2DNode53 * tex2D( _Foam, rotator85 ) ) ));
			float smoothstepResult80 = smoothstep( _TEmp.x , _TEmp.y , smoothstepResult55.r);
			float clampResult82 = clamp( ( smoothstepResult80 * 1000.0 ) , 0.0 , 1.0 );
			float Debug51 = ( clampResult82 * _WaveOpacity );
			float clampResult77 = clamp( Debug51 , 0.0 , 1.0 );
			o.Emission = ( _BaseColor + Edge41 + clampResult77 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17200
0;73;1924;928;-422.885;-753.925;1.3;True;False
Node;AmplifyShaderEditor.CommentaryNode;43;-1473.873,1101.677;Inherit;False;2340.896;880.2334;;27;13;15;16;12;9;10;14;11;8;7;17;18;38;19;20;21;39;40;22;41;23;25;44;64;67;68;69;Edge;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;16;-1273.565,1666.925;Inherit;False;Property;_FoamPan2;FoamPan2;7;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;93;879.0074,1001.538;Inherit;False;Property;_WaterDirectionScale;WaterDirectionScale;17;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;15;-1280.565,1535.925;Inherit;False;Property;_FoamPan1;FoamPan1;6;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;68;-910.0416,1282.866;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;89;1156.218,998.6796;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;92;1122.218,1091.279;Inherit;False;Property;_WaterDirection;WaterDirection;16;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;67;-922.2766,1137.883;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-765.3445,1300.997;Inherit;False;vel2;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;63;1112.396,1437.402;Inherit;False;Property;_FoamTimescale;Foam Timescale;12;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;54;1133.226,1216.359;Inherit;False;0;58;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;1415.218,1074.279;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;87;1571.218,1213.279;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;64;-780.9065,1133.465;Inherit;False;vel1;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;62;1316.496,1336.002;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;1418.638,1767.328;Inherit;False;69;vel2;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-1555.306,-95.52911;Inherit;False;Property;_EdgeDistance;Edge Distance;0;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;61;1652.169,1405.609;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;1428.81,1434.631;Inherit;False;64;vel1;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;86;1645.45,1824.075;Inherit;False;Constant;_Float0;Float 0;16;0;Create;True;0;0;False;0;3.141593;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;60;1646.283,1542.957;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;85;1701.45,1700.075;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;58;1102.19,1559.892;Inherit;True;Property;_Foam;Foam;11;0;Create;True;0;0;False;0;732d785c5a13e9242a8f461521ba2964;732d785c5a13e9242a8f461521ba2964;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DepthFade;1;-1346.639,-179.5371;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1423.873,1816.48;Inherit;False;Property;_TimeScale;TimeScale;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;12;-1233.873,1821.48;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1290.184,1403.839;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;3;-1143.886,-63.91845;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;53;1931.453,1274.131;Inherit;True;Property;_FoamTexture;FoamTexture;11;0;Create;True;0;0;False;0;-1;732d785c5a13e9242a8f461521ba2964;732d785c5a13e9242a8f461521ba2964;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;59;1928.396,1509.216;Inherit;True;Property;_TextureSample0;Texture Sample 0;13;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-1423.15,65.7245;Inherit;False;Property;_EdgePower;Edge Power;2;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;2276.855,1538.406;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;10;-1043.173,1455.797;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1007.178,1810.679;Inherit;False;Property;_FoamNoiseScale;FoamNoiseScale;5;0;Create;True;0;0;False;0;4.34;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-519.5287,-567.8994;Inherit;False;Property;_BaseEdgeDistance;Base Edge Distance;1;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;11;-1029.624,1609.276;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1117.15,56.7245;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;7;-773.7366,1438.111;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;78;1375.176,837.7974;Inherit;False;Property;_WavesSmoothstep;WavesSmoothstep;14;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.NoiseGeneratorNode;8;-773.2368,1723.911;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;2316.431,1328.254;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;31;-310.8614,-651.9074;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;6;-959.5117,96.55976;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;55;1703.621,814.8666;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0754717,0.0754717,0.0754717,0;False;2;COLOR;0.08490568,0.08490568,0.08490568,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;35;-108.1091,-536.2888;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-387.3725,-406.6457;Inherit;False;Property;_BaseEdgePower;Base Edge Power;3;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-516.0459,1628.083;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-769.1101,138.4391;Inherit;False;SecondaryEdge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;73;2046.646,696.2382;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-81.37253,-415.6457;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;18;-258.2436,1590.355;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;-296.0988,1387.147;Inherit;False;37;SecondaryEdge;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;79;2308.262,880.4581;Inherit;False;Property;_TEmp;TEmp;15;0;Create;True;0;0;False;0;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SmoothstepOpNode;80;2491.161,749.3581;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;33;76.26596,-375.8105;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-64.21494,1423.486;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;2723.525,728.0001;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;1000;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;285.8395,-315.7767;Inherit;False;MainEdge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;20;141.3733,1513.513;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;292.5693,1592.654;Inherit;False;36;MainEdge;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;21;101.6618,1321.066;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;2091.983,463.5859;Inherit;False;Property;_WaveOpacity;WaveOpacity;13;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;82;2837.927,568.0998;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;2302.337,568.9167;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;204.0282,1151.677;Inherit;False;Property;_StylizerMin;StylizerMin;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;181.7523,1247.382;Inherit;False;Property;_StylizerMax;StylizerMax;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;464.5692,1451.654;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;2411.144,449.1031;Inherit;False;Debug;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;22;386.6617,1230.065;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;41;624.0224,1258.803;Inherit;False;Edge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;76;-367.5908,345.8259;Inherit;False;51;Debug;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-320.9181,182.3787;Inherit;False;41;Edge;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;26;-315.4584,-136.2106;Inherit;False;Property;_BaseColor;Base Color;10;0;Create;True;0;0;False;0;0,0.5669932,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;77;-128.5908,365.8259;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;1376.579,485.7333;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;559.6677,637.0825;Inherit;True;44;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;48;1133.597,656.5189;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.03;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-131.1738,93.72525;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;49;839.6058,378.0748;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-66.95831,1693.618;Inherit;False;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;1828.098,653.3842;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;47;850.1201,651.8869;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.31;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;12.80859,-111.0078;Float;False;True;2;ASEMaterialInspector;0;0;Standard;Remasu/Casper/CartoonWater;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;68;0;16;1
WireConnection;68;1;16;2
WireConnection;89;0;93;0
WireConnection;67;0;15;1
WireConnection;67;1;15;2
WireConnection;69;0;68;0
WireConnection;90;0;89;0
WireConnection;90;1;92;0
WireConnection;87;0;54;0
WireConnection;87;2;90;0
WireConnection;64;0;67;0
WireConnection;62;0;63;0
WireConnection;61;0;87;0
WireConnection;61;2;71;0
WireConnection;61;1;62;0
WireConnection;60;0;87;0
WireConnection;60;2;70;0
WireConnection;60;1;62;0
WireConnection;85;0;61;0
WireConnection;85;2;86;0
WireConnection;1;0;2;0
WireConnection;12;0;13;0
WireConnection;3;0;1;0
WireConnection;53;0;58;0
WireConnection;53;1;60;0
WireConnection;59;0;58;0
WireConnection;59;1;85;0
WireConnection;72;0;53;0
WireConnection;72;1;59;0
WireConnection;10;0;9;0
WireConnection;10;2;15;0
WireConnection;10;1;12;0
WireConnection;11;0;9;0
WireConnection;11;2;16;0
WireConnection;11;1;12;0
WireConnection;4;0;3;0
WireConnection;4;1;5;0
WireConnection;7;0;10;0
WireConnection;7;1;14;0
WireConnection;8;0;11;0
WireConnection;8;1;14;0
WireConnection;84;0;53;0
WireConnection;84;1;72;0
WireConnection;31;0;30;0
WireConnection;6;0;4;0
WireConnection;55;0;84;0
WireConnection;55;1;78;1
WireConnection;55;2;78;2
WireConnection;35;0;31;0
WireConnection;17;0;7;0
WireConnection;17;1;8;0
WireConnection;37;0;6;0
WireConnection;73;0;55;0
WireConnection;32;0;35;0
WireConnection;32;1;34;0
WireConnection;18;0;17;0
WireConnection;80;0;73;0
WireConnection;80;1;79;1
WireConnection;80;2;79;2
WireConnection;33;0;32;0
WireConnection;19;0;38;0
WireConnection;19;1;18;0
WireConnection;83;0;80;0
WireConnection;36;0;33;0
WireConnection;20;0;19;0
WireConnection;21;0;20;0
WireConnection;82;0;83;0
WireConnection;74;0;82;0
WireConnection;74;1;75;0
WireConnection;40;0;21;0
WireConnection;40;1;39;0
WireConnection;51;0;74;0
WireConnection;22;0;40;0
WireConnection;22;1;23;0
WireConnection;22;2;25;0
WireConnection;41;0;22;0
WireConnection;77;0;76;0
WireConnection;50;0;49;0
WireConnection;50;1;48;0
WireConnection;48;0;47;0
WireConnection;27;0;26;0
WireConnection;27;1;42;0
WireConnection;27;2;77;0
WireConnection;49;0;46;0
WireConnection;44;0;18;0
WireConnection;56;0;50;0
WireConnection;56;1;55;0
WireConnection;47;0;46;0
WireConnection;0;0;26;0
WireConnection;0;2;27;0
ASEEND*/
//CHKSM=71EE2ED716DA72C377259ADAF858BBBC897CEC5F