# ComPortUygulamasi
c# form uygulaması

```C#
#include "stm32f4xx.h"
#include "stm32f4_discovery.h"
#include "stm32f4xx_usart.h"
#include <stdbool.h>

GPIO_InitTypeDef GPIO_InitStruct;
USART_InitTypeDef USART_InitStruct;
NVIC_InitTypeDef NVIC_InitStruct;
TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
GPIO_InitTypeDef GPIO_InitLed;


bool press = 0;
bool Uart_Gonder_Flag = 1 ;

char rx_buffer[5];   // Local holding buffer to build line
 int rx_index = 0;
uint16_t   c = 0;
int16_t oku_uart;
int16_t cizgi_sayisi;
uint16_t counter_timer = 0;
uint16_t counter_Can = 0;
bool Can_Gonder_Flag = 0 ;

bool yon =0;
bool start = 0 ;
int16_t deneme2 = 0;
int cnt1 = 0;
int i = 0 ;
int counter = 0;
int16_t deneme = 0 ;

bool SendFlag = 0;
bool CiftCizgi = 0 ;

volatile char line_buffer[15 + 1]; // Holding buffer with space for terminating NUL
volatile int line_valid = 0;


void USART_puts(USART_TypeDef* USARTx, volatile char *s)
{

	while(*s)
	{
		// wait until data register is empty
		while( !(USARTx->SR & 0x00000040) );
		USART_SendData(USARTx, *s);
		*s++;
	}
}


int main(void)
{

 	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOC, ENABLE);
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM3, ENABLE);
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOA,ENABLE);
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOG,ENABLE);
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOB,ENABLE);
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOD, ENABLE);
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOG, ENABLE);
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOB,ENABLE);


	GPIO_InitLed.GPIO_Pin=GPIO_Pin_0;
	GPIO_InitLed.GPIO_Mode=GPIO_Mode_IN;
	GPIO_InitLed.GPIO_OType=GPIO_OType_PP;
	GPIO_InitLed.GPIO_PuPd=GPIO_PuPd_DOWN;
	GPIO_InitLed.GPIO_Speed=GPIO_Speed_50MHz;
	GPIO_Init(GPIOA,&GPIO_InitLed);


	TIM_TimeBaseStructure.TIM_Period = 3999;
	TIM_TimeBaseStructure.TIM_Prescaler = 17;
	TIM_TimeBaseStructure.TIM_ClockDivision = 0;
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;
	TIM_TimeBaseInit(TIM3, &TIM_TimeBaseStructure);// TİM3 İÇİN

	TIM_ARRPreloadConfig(TIM3, ENABLE);
	TIM_Cmd(TIM3, ENABLE);
	TIM_ITConfig(TIM3,TIM_IT_Update, ENABLE);

	NVIC_InitStruct.NVIC_IRQChannel = TIM3_IRQn;
	NVIC_InitStruct.NVIC_IRQChannelPreemptionPriority  = 0x00;
	NVIC_InitStruct.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStruct);


	NVIC_Config();

	//LED CONFIG

	/* Configure PD12, PD13, PD14 and PD15 in output pushpull mode */
	GPIO_InitStruct.GPIO_Pin = GPIO_Pin_6 ;
	GPIO_InitStruct.GPIO_Mode = GPIO_Mode_OUT;
	GPIO_InitStruct.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStruct.GPIO_Speed = GPIO_Speed_100MHz;
	GPIO_InitStruct.GPIO_PuPd = GPIO_PuPd_NOPULL;
	GPIO_Init(GPIOG, &GPIO_InitStruct);

	GPIO_InitStruct.GPIO_Pin = GPIO_Pin_4 ;
	GPIO_InitStruct.GPIO_Mode = GPIO_Mode_OUT;
	GPIO_InitStruct.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStruct.GPIO_Speed = GPIO_Speed_100MHz;
	GPIO_InitStruct.GPIO_PuPd = GPIO_PuPd_NOPULL;
	GPIO_Init(GPIOD, &GPIO_InitStruct);


	GPIO_InitStruct.GPIO_Pin = GPIO_Pin_1;
	GPIO_InitStruct.GPIO_Mode = GPIO_Mode_AN;
	GPIO_InitStruct.GPIO_PuPd = GPIO_PuPd_NOPULL ;
	GPIO_Init(GPIOB, &GPIO_InitStruct);

	 //USART CONF

	 GPIO_InitStruct.GPIO_Pin = GPIO_Pin_14 | GPIO_Pin_9;
	 GPIO_InitStruct.GPIO_Mode = GPIO_Mode_AF;
	 GPIO_InitStruct.GPIO_OType = GPIO_OType_PP;
	 GPIO_InitStruct.GPIO_PuPd = GPIO_PuPd_UP;
	 GPIO_InitStruct.GPIO_Speed = GPIO_Speed_100MHz;
	 GPIO_Init(GPIOG, &GPIO_InitStruct);

	 GPIO_PinAFConfig(GPIOG, GPIO_PinSource14, GPIO_AF_USART6);//TX
	 GPIO_PinAFConfig(GPIOG, GPIO_PinSource9, GPIO_AF_USART6);//RX

	 RCC_APB2PeriphClockCmd(RCC_APB2Periph_USART6, ENABLE);

	 USART_InitStruct.USART_BaudRate = 115200;
	 USART_InitStruct.USART_HardwareFlowControl = USART_HardwareFlowControl_None;
	 USART_InitStruct.USART_Mode = USART_Mode_Rx |USART_Mode_Tx ;
	 USART_InitStruct.USART_Parity = USART_Parity_No;
	 USART_InitStruct.USART_StopBits = USART_StopBits_1;
	 USART_InitStruct.USART_WordLength = USART_WordLength_8b;
	 USART_Init(USART6, &USART_InitStruct);
	 USART_Cmd(USART6, ENABLE);

	 USART_ITConfig(USART6, USART_IT_RXNE, ENABLE);

	 NVIC_InitStruct.NVIC_IRQChannel = USART6_IRQn;
	 NVIC_InitStruct.NVIC_IRQChannelCmd = ENABLE;
	 NVIC_InitStruct.NVIC_IRQChannelPreemptionPriority = 0;
	 NVIC_InitStruct.NVIC_IRQChannelSubPriority = 0;
	 NVIC_Init(&NVIC_InitStruct);

	 //USART_SendData(USART6, 12);

	 USART_puts(USART6, "12\r\n"); // just send a message to indicate that it works


  while (1)
  {
	  if(SendFlag)
	  {

		  USART_SendData(USART6, 5+48);
		  SendFlag = 0;

	  }
  }
}

void USART6_IRQHandler(void)
{
	if(USART_GetITStatus(USART6, USART_IT_RXNE) != RESET)
    {

	     char rx = (char) USART_ReceiveData(USART6);

	     if ( (rx == '\n'))
	     {

	    	 	 i = 1;
	    	 	 cnt1 = 0 ;
	     }


		 if(i == 1)
		 {
			 rx_buffer[rx_index++] = rx; // Copy to buffer and increment
			 cnt1++;

			 if ( cnt1 >= 5  )
			 {
				 i = 0 ;
				 oku_uart = (rx_buffer[1]-48)*100 + (rx_buffer[2]-48)*10 + (rx_buffer[3]-48);
				// cizgi_sayisi = (rx_buffer[4]) - 48;
				 rx_index=0;
}

	     	 }
    }
}

```
