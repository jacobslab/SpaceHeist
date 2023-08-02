// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/Water"
{
	Properties
	{
		_WaterNormal("Water Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( 0 , 1)) = 0
		_WaterSpecular("Water Specular", Range( 0 , 1)) = 0
		_WaterSmoothness("Water Smoothness", Range( 0 , 1)) = 0
		_WaterColor("Water Color", Color) = (0.4004984,0.6405662,0.6792453,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular alpha:fade keepalpha 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _WaterNormal;
		uniform float _NormalScale;
		uniform float4 _WaterNormal_ST;
		uniform half4 _WaterColor;
		uniform float _WaterSpecular;
		uniform float _WaterSmoothness;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv0_WaterNormal = i.uv_texcoord * _WaterNormal_ST.xy + _WaterNormal_ST.zw;
			half2 panner22 = ( 1.0 * _Time.y * float2( -0.03,0 ) + uv0_WaterNormal);
			half2 panner19 = ( 1.0 * _Time.y * float2( 0.04,0.04 ) + uv0_WaterNormal);
			o.Normal = BlendNormals( UnpackScaleNormal( tex2D( _WaterNormal, panner22 ), _NormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, panner19 ), _NormalScale ) );
			o.Albedo = _WaterColor.rgb;
			float3 temp_cast_1 = (_WaterSpecular).xxx;
			o.Specular = temp_cast_1;
			o.Smoothness = _WaterSmoothness;
			o.Alpha = _WaterColor.a;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17200
1927;196;1066;987;-698.6147;1165.932;1.3;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-237.8175,-846.4249;Inherit;False;0;17;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;22;69.67883,-882.4233;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;19;71.97889,-739.477;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.04,0.04;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;48;58.15997,-604.0038;Float;False;Property;_NormalScale;Normal Scale;1;0;Create;True;0;0;False;0;0;0.343;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;17;492.6765,-647.1531;Inherit;True;Property;_WaterNormal;Water Normal;0;0;Create;True;0;0;False;0;-1;None;2c5e90c73951f064eb84fa634827dc10;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;494.1461,-913.2703;Inherit;True;Property;_Normal2;Normal2;0;0;Create;True;0;0;False;0;-1;None;None;True;0;True;bump;Auto;True;Instance;17;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendNormalsNode;24;905.852,-727.0548;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;104;1302.234,-614.9037;Float;False;Property;_WaterSpecular;Water Specular;2;0;Create;True;0;0;False;0;0;0.759;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;1287.986,-510.2036;Float;False;Property;_WaterSmoothness;Water Smoothness;3;0;Create;True;0;0;False;0;0;0.892;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;172;1289.222,-860.9869;Inherit;False;Property;_WaterColor;Water Color;4;0;Create;True;0;0;False;0;0.4004984,0.6405662,0.6792453,0;0.4141153,0.5932717,0.6226415,0.509804;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1838.601,-748.1998;Half;False;True;2;ASEMaterialInspector;0;0;StandardSpecular;Custom/Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;21;0
WireConnection;19;0;21;0
WireConnection;17;1;19;0
WireConnection;17;5;48;0
WireConnection;23;1;22;0
WireConnection;23;5;48;0
WireConnection;24;0;23;0
WireConnection;24;1;17;0
WireConnection;0;0;172;0
WireConnection;0;1;24;0
WireConnection;0;3;104;0
WireConnection;0;4;26;0
WireConnection;0;9;172;4
ASEEND*/
//CHKSM=0E1BF9E99EAFCF62F658BEF01EC2F2AC95E95B5A