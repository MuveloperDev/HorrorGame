Shader "Unlit/UI_S_Dissolve"
{
    Properties
    {
        // 셰이더 프로퍼티가 각 렌더러(예: 각 오브젝트 인스턴스)에 대해 개별적으로 설정될 수 있음을 나타낸다.
        //  메테리얼의 텍스처 등을 각 오브젝트마다 다르게 설정하고 싶을 때 사용
        // 모든 오브젝트가 동일한 메테리얼을 공유하더라도, 각 오브젝트마다 다른 텍스처를 사용할 수 있습니다.
        [PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}   // 주 텍스처
        _Color ("Tint", Color) = (1,1,1,1)

        // Stencil Buffer
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        // 유니티 UI 시스템에서 알파 클리핑을 사용하는 기능을 활성화하기 위한 셰이더 프레그먼트
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        [Header(Dissolve)]
        _NoiseTex("Noise Texture (A)", 2D) = "white" {} 
    }
    SubShader
    {
        // SubShader 블록은 유니티 셰이더의 핵심 부분 중 하나로, 
        //여러 SubShader를 정의하여 하드웨어 및 렌더링 상황에 따라 최적의 셰이더를 선택할 수 있게 합니다.
        // 각 SubShader 블록은 여러 Pass로 구성될 수 있으며, 각 Pass는 실제 렌더링을 수행하는 단계

        /*
        하드웨어 호환성: 여러 SubShader를 사용하여 다양한 하드웨어 및 렌더링 경로를 지원할 수 있습니다. 
        유니티는 주어진 환경에 가장 적합한 SubShader를 자동으로 선택합니다.

        렌더링 설정: 각 SubShader는 렌더링 상태를 설정하고, 렌더링 순서, 블렌딩 모드, 스텐실 설정 등을 정의합니다.

        다양한 렌더링 기술 지원: SubShader를 사용하여 같은 셰이더 코드 내에서 다양한 렌더링 기술을 지원할 수 있습니다. 
        예를 들어, 고급 그래픽 카드와 낮은 성능의 그래픽 카드에 대해 다른 SubShader를 정의할 수 있습니다.
        */


        Tags 
        {
            // Tags: 렌더링 순서 및 유형을 정의
            "Queue"="Transparent" // "Queue"="Transparent"는 투명한 오브젝트를 렌더링 큐에서 처리
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

        Cull Off                            // 백페이스 컬링을 비활성화합니다. 이는 모든 면을 렌더링하도록 합니다
        Lighting Off                        //  조명을 비활성화합니다. UI 요소나 단순한 효과에 적합
        ZWrite Off                          // 깊이 버퍼에 쓰기를 비활성화합니다. 이는 투명한 오브젝트에 적합
        ZTest [unity_GUIZTestMode]          // 깊이 테스트 모드를 설정
        Blend SrcAlpha OneMinusSrcAlpha     // 블렌딩 모드를 설정합니다. 이는 반투명 렌더링을 위해 설정
        ColorMask [_ColorMask]              // 컬러 채널 마스크를 설정

        Pass
        {
            Name "Default"

            CGPROGRAM
            // #pragma 지시문은 셰이더 컴파일러에게 특정 설정을 전달합니다.
            #pragma vertex vert // 정점 셰이더
            #pragma fragment frag // 프레그먼트 셰이더
            #pragma target 2.0  // 셰이더 모델 2.0을 타겟으로 하도록 설정.
            // make fog work
            // 다양한 컴파일 옵션을 지원하도록 한다. 
            // 여기서는 포그와 UI 알파 클립 옵션을 컴파일한다.
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            // 다양한 셰이더 기능을 컴파일 하도록 한다.
            // 이 경우 색상 추가, 색상 제거, 색상 설정 기능이 있다.
            #pragma shader_feature __ UI_COLOR_ADD UI_COLOR_SUB UI_COLOR_SET

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID

                float2 uv1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            
                fixed4 effectFactor : TEXCOORD2;
                fixed4 effectFactor2 : TEXCOORD3;
            };

            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            sampler2D _MainTex;
            sampler2D _NoiseTex;

            // 패킹된 UIVertex.uv1에서 값을 받아 언패킹후 반환하는 함수.
            fixed4 UnpackToVec4(float value)
            {
                const int PACKER_STEP = 64;
                const int PRECISION = PACKER_STEP - 1;
                fixed4 color;

                color.r = (value % PACKER_STEP) / PRECISION;
                value = floor(value / PACKER_STEP);

                color.g = (value % PACKER_STEP) / PRECISION;
                value = floor(value / PACKER_STEP);

                color.b = (value % PACKER_STEP) / PRECISION;
                value = floor(value / PACKER_STEP);

                color.a = (value % PACKER_STEP) / PRECISION;
                return color;
            }

            fixed4 ApplyColorEffect(fixed4 color, fixed4 factor)
            {
                #ifdef UI_COLOR_SET 
                color.rgb = lerp(color.rgb, factor.rgb, factor.a);

                #elif UI_COLOR_ADD 
                color.rgb += factor.rgb * factor.a;

                #elif UI_COLOR_SUB 
                color.rgb -= factor.rgb * factor.a;

                #else
                color.rgb = lerp(color.rgb, color.rgb * factor.rgb, factor.a);
                #endif

                return color;
            }

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = IN.vertex;

                OUT.vertex = UnityObjectToClipPos(IN.vertex);

                OUT.texcoord = IN.texcoord;
            
                OUT.color = IN.color * _Color;

                //xy: Noize uv, z: Dissolve factor, w: width
                OUT.effectFactor = UnpackToVec4(IN.uv1.x);
                //xyz: color, w: softness
                OUT.effectFactor2 = UnpackToVec4(IN.uv1.y);
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

                float cutout = tex2D(_NoiseTex, IN.effectFactor.xy).a;
                fixed factor = cutout - IN.effectFactor.z;

                #ifdef UNITY_UI_ALPHACLIP
                clip (min(color.a - 0.01, factor));
                #endif

                fixed edgeLerp = step(cutout, color.a) * saturate((IN.effectFactor.w/4 - factor)*16/ IN.effectFactor2.w);
                color = ApplyColorEffect(color, fixed4(IN.effectFactor2.rgb, edgeLerp));
                color.a *= saturate((factor)*32/ IN.effectFactor2.w);

                return color;
            }
            ENDCG
        }
    }
}
