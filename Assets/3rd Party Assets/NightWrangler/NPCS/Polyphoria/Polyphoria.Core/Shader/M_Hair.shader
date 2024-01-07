// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "M_Hair"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_MSO("MSO", 2D) = "white" {}
		_TintMask("Tint Mask", 2D) = "white" {}
		_RSmoothnessMultiplier("R: Smoothness Multiplier", Float) = 1
		_GSmoothnessMultiplier("G: Smoothness Multiplier", Float) = 1
		_BSmoothnessMultiplier("B: Smoothness Multiplier", Float) = 1
		_ASmoothnessMultiplier("A: Smoothness Multiplier", Float) = 1
		[HDR]_RPrimary("R: Primary", Color) = (1,0,0,0)
		[HDR]_GDetails1("G: Details 1", Color) = (0,1,0,0)
		[HDR]_BDetails2("B: Details 2", Color) = (0,0,1,0)
		[HDR]_ADetails3Skin("A: Details 3 / Skin", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _TintMask;
		uniform float4 _TintMask_ST;
		uniform float4 _RPrimary;
		uniform float4 _GDetails1;
		uniform float4 _BDetails2;
		uniform float4 _ADetails3Skin;
		uniform sampler2D _MSO;
		uniform float4 _MSO_ST;
		uniform float _RSmoothnessMultiplier;
		uniform float _GSmoothnessMultiplier;
		uniform float _BSmoothnessMultiplier;
		uniform float _ASmoothnessMultiplier;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			float4 temp_output_21_0_g7 = tex2DNode1;
			float3 desaturateInitialColor22_g7 = temp_output_21_0_g7.xyz;
			float desaturateDot22_g7 = dot( desaturateInitialColor22_g7, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar22_g7 = lerp( desaturateInitialColor22_g7, desaturateDot22_g7.xxx, 1.0 );
			float2 uv_TintMask = i.uv_texcoord * _TintMask_ST.xy + _TintMask_ST.zw;
			float4 tex2DNode3 = tex2D( _TintMask, uv_TintMask );
			float4 break26_g7 = tex2DNode3;
			float4 temp_output_27_0_g7 = _RPrimary;
			float temp_output_8_0_g7 = ( break26_g7.r * temp_output_27_0_g7.a );
			float4 temp_output_28_0_g7 = _GDetails1;
			float temp_output_10_0_g7 = ( break26_g7.g * temp_output_28_0_g7.a );
			float4 temp_output_29_0_g7 = _BDetails2;
			float temp_output_12_0_g7 = ( break26_g7.b * temp_output_29_0_g7.a );
			float4 temp_output_30_0_g7 = _ADetails3Skin;
			float temp_output_14_0_g7 = ( break26_g7.a * temp_output_30_0_g7.a );
			float4 lerpResult24_g7 = lerp( temp_output_21_0_g7 , ( float4( desaturateVar22_g7 , 0.0 ) * ( ( ( ( temp_output_8_0_g7 * temp_output_27_0_g7 ) + ( temp_output_10_0_g7 * temp_output_28_0_g7 ) ) + ( temp_output_12_0_g7 * temp_output_29_0_g7 ) ) + ( temp_output_14_0_g7 * temp_output_30_0_g7 ) ) ) , ( ( ( temp_output_8_0_g7 + temp_output_10_0_g7 ) + temp_output_12_0_g7 ) + temp_output_14_0_g7 ));
			o.Albedo = lerpResult24_g7.rgb;
			float2 uv_MSO = i.uv_texcoord * _MSO_ST.xy + _MSO_ST.zw;
			float4 tex2DNode4 = tex2D( _MSO, uv_MSO );
			o.Metallic = tex2DNode4.r;
			float4 temp_cast_5 = (tex2DNode4.g).xxxx;
			float4 temp_output_19_0_g9 = temp_cast_5;
			float4 break2_g9 = tex2DNode3;
			float4 lerpResult18_g9 = lerp( temp_output_19_0_g9 , ( temp_output_19_0_g9 * ( ( ( ( break2_g9.r * _RSmoothnessMultiplier ) + ( break2_g9.g * _GSmoothnessMultiplier ) ) + ( break2_g9.b * _BSmoothnessMultiplier ) ) + ( break2_g9.a * _ASmoothnessMultiplier ) ) ) , ( ( ( break2_g9.r + break2_g9.g ) + break2_g9.b ) + break2_g9.a ));
			o.Smoothness = lerpResult18_g9.r;
			o.Occlusion = tex2DNode4.b;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17000
-1699;224;1275;946;770.2573;484.8789;1.879249;True;True
Node;AmplifyShaderEditor.SamplerNode;1;-256,-208;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;921627d1f99ceef439273015899e234f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-256,-1;Float;True;Property;_TintMask;Tint Mask;4;0;Create;True;0;0;False;0;None;a3eb862ead87505418053eb58d4f61be;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-256,206.7;Float;True;Property;_MSO;MSO;3;0;Create;True;0;0;False;0;None;131b3590b4f85e64e94e8217bb39d935;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;27;-211.5762,-673.2794;Float;False;Property;_RPrimary;R: Primary;9;1;[HDR];Create;True;0;0;False;0;1,0,0,0;1.624505,1.503463,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;28;-205.2022,-507.5521;Float;False;Property;_GDetails1;G: Details 1;10;1;[HDR];Create;True;0;0;False;0;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;29;30.64025,-650.3323;Float;False;Property;_BDetails2;B: Details 2;11;1;[HDR];Create;True;0;0;False;0;0,0,1,0;0,0,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;30;25.54098,-484.605;Float;False;Property;_ADetails3Skin;A: Details 3 / Skin;12;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-210.9515,452.7779;Float;False;Property;_RSmoothnessMultiplier;R: Smoothness Multiplier;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-211.067,535.6442;Float;False;Property;_GSmoothnessMultiplier;G: Smoothness Multiplier;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-222.4249,625.9528;Float;False;Property;_BSmoothnessMultiplier;B: Smoothness Multiplier;7;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-205.8522,725.0753;Float;False;Property;_ASmoothnessMultiplier;A: Smoothness Multiplier;8;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;33;196.8406,-228.97;Float;False;RGBA_Tint;-1;;7;74a2eda5d0c8f5149ac824704f4c7a12;0;6;21;FLOAT4;0,0,0,1;False;1;COLOR;0,0,0,0;False;27;COLOR;0,0,0,0;False;28;COLOR;0,0,0,0;False;29;COLOR;0,0,0,0;False;30;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;26;257.5291,320.0565;Float;False;RGBA_Multiplier;-1;;9;bf65d87512a6cd64fbdc1ac41af05dbf;0;6;19;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;15;273.3891,552.7795;Float;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;None;bd66352fce8c73c4fb1449775be9c290;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;715,-192;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;M_Hair;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;33;21;1;0
WireConnection;33;1;3;0
WireConnection;33;27;27;0
WireConnection;33;28;28;0
WireConnection;33;29;29;0
WireConnection;33;30;30;0
WireConnection;26;19;4;2
WireConnection;26;1;3;0
WireConnection;26;3;11;0
WireConnection;26;4;12;0
WireConnection;26;5;13;0
WireConnection;26;6;14;0
WireConnection;0;0;33;0
WireConnection;0;1;15;0
WireConnection;0;3;4;1
WireConnection;0;4;26;0
WireConnection;0;5;4;3
WireConnection;0;10;1;4
ASEEND*/
//CHKSM=4F1009078FB6C248CDFC579518679BFB61FA803F