import { CanActivateFn } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {

  
  // Überprüfe, ob ein Token im localStorage vorhanden ist
  const token = localStorage.getItem('token');
  if (token) {
    // Benutzer ist authentifiziert
    return true;
  } else {
    // Benutzer ist nicht authentifiziert, weiterleiten zur Anmeldeseite
    // Hier kannst du die Weiterleitung anpassen, je nachdem, wie deine Anmeldeseite heißt
    window.location.href = '/login'; // Beispiel: Weiterleitung zur Anmeldeseite
    return false;
  }
};
