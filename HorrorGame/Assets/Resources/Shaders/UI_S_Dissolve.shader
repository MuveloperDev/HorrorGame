Shader "Unlit/UI_S_Dissolve"
{
    Properties
    {
        // ���̴� ������Ƽ�� �� ������(��: �� ������Ʈ �ν��Ͻ�)�� ���� ���������� ������ �� ������ ��Ÿ����.
        //  ���׸����� �ؽ�ó ���� �� ������Ʈ���� �ٸ��� �����ϰ� ���� �� ���
        // ��� ������Ʈ�� ������ ���׸����� �����ϴ���, �� ������Ʈ���� �ٸ� �ؽ�ó�� ����� �� �ֽ��ϴ�.
        [PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}   // �� �ؽ�ó
        _Color ("Tint", Color) = (1,1,1,1)

        // Stencil Buffer
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        // ����Ƽ UI �ý��ۿ��� ���� Ŭ������ ����ϴ� ����� Ȱ��ȭ�ϱ� ���� ���̴� �����׸�Ʈ
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        [Header(Dissolve)]
        _NoiseTex("Noise Texture (A)", 2D) = "white" {} 
    }
    SubShader
    {
        // SubShader ����� ����Ƽ ���̴��� �ٽ� �κ� �� �ϳ���, 
        //���� SubShader�� �����Ͽ� �ϵ���� �� ������ ��Ȳ�� ���� ������ ���̴��� ������ �� �ְ� �մϴ�.
        // �� SubShader ����� ���� Pass�� ������ �� ������, �� Pass�� ���� �������� �����ϴ� �ܰ�

        /*
        �ϵ���� ȣȯ��: ���� SubShader�� ����Ͽ� �پ��� �ϵ���� �� ������ ��θ� ������ �� �ֽ��ϴ�. 
        ����Ƽ�� �־��� ȯ�濡 ���� ������ SubShader�� �ڵ����� �����մϴ�.

        ������ ����: �� SubShader�� ������ ���¸� �����ϰ�, ������ ����, ���� ���, ���ٽ� ���� ���� �����մϴ�.

        �پ��� ������ ��� ����: SubShader�� ����Ͽ� ���� ���̴� �ڵ� ������ �پ��� ������ ����� ������ �� �ֽ��ϴ�. 
        ���� ���, ��� �׷��� ī��� ���� ������ �׷��� ī�忡 ���� �ٸ� SubShader�� ������ �� �ֽ��ϴ�.
        */


        Tags 
        {
            // Tags: ������ ���� �� ������ ����
            "Queue"="Transparent" // "Queue"="Transparent"�� ������ ������Ʈ�� ������ ť���� ó��
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

        Cull Off                            // �����̽� �ø��� ��Ȱ��ȭ�մϴ�. �̴� ��� ���� �������ϵ��� �մϴ�
        Lighting Off                        //  ������ ��Ȱ��ȭ�մϴ�. UI ��ҳ� �ܼ��� ȿ���� ����
        ZWrite Off                          // ���� ���ۿ� ���⸦ ��Ȱ��ȭ�մϴ�. �̴� ������ ������Ʈ�� ����
        ZTest [unity_GUIZTestMode]          // ���� �׽�Ʈ ��带 ����
        Blend SrcAlpha OneMinusSrcAlpha     // ���� ��带 �����մϴ�. �̴� ������ �������� ���� ����
        ColorMask [_ColorMask]              // �÷� ä�� ����ũ�� ����

        Pass
        {
            Name "Default"

            CGPROGRAM
            // #pragma ���ù��� ���̴� �����Ϸ����� Ư�� ������ �����մϴ�.
            #pragma vertex vert // ���� ���̴�
            #pragma fragment frag // �����׸�Ʈ ���̴�
            #pragma target 2.0  // ���̴� �� 2.0�� Ÿ������ �ϵ��� ����.
            // make fog work
            // �پ��� ������ �ɼ��� �����ϵ��� �Ѵ�. 
            // ���⼭�� ���׿� UI ���� Ŭ�� �ɼ��� �������Ѵ�.
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            // �پ��� ���̴� ����� ������ �ϵ��� �Ѵ�.
            // �� ��� ���� �߰�, ���� ����, ���� ���� ����� �ִ�.
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

            // ��ŷ�� UIVertex.uv1���� ���� �޾� ����ŷ�� ��ȯ�ϴ� �Լ�.
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
