#include "stdafx.h"
#include <stdlib.h>
#include "SDL.h"

#include "SerialHcPolling.h"
#include "GlobalVars.h"

void PollForHcEvents()
{
  // This is a simulation. The controls are read from the keyboard
  // and then sent to the SerialHcLoop which reads them from a buffer and actions them as it would normally
  SDL_Event event;

  while(SDL_PollEvent( &event ))
  {
    /* We are only worried about SDL_KEYDOWN and SDL_KEYUP events */
    switch( event.type )
	{
      case SDL_KEYDOWN:
		SDLKey key;
		key = event.key.keysym.sym;
		
		EnterCriticalSection(&s_SyncRoot);
		s_KeybordInput.push(SDLKey(key));
		LeaveCriticalSection(&s_SyncRoot);	
        
        break;

      case SDL_KEYUP:
        break;

      default:
        break;
    }
  }	
}


void SendHcAdvrCommand(HcAdvrCommandType cmd)
{
	// TODO:
}
