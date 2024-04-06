import { Injectable, Input } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthentifikationService {
  @Input() password: string = '';
  @Input() email: string = '';
  @Input() registerpassword: string = '';
  @Input() registeremail: string = '';
  @Input() name: string = '';
  datas: any = '';
  emailEndings: string[] = ['.com', '.net', '.org', '.edu', '.gov'];
  url: string = 'https://localhost:7045';
  errorResponse: boolean = false;

  constructor(public client: HttpClient, private router: Router) {
    this.client = client;
    this.router = router;
  }

  isValidEmail(email: string): boolean {
    const emailRegex: RegExp = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  SendLoginDatas() {
    if (this.isValidEmail(this.email)) {
      const body = { email: this.email, password: this.password };
      this.client.post(this.url + '/login', body).subscribe(
        (response) => {
          console.log('Response:', response);
          const token = (response as any).token;
          if (token) {
            // Speichere den Token irgendwo, z.B. im localStorage
            localStorage.setItem('token', token);
            // Weiterleitung oder andere Aktionen nach erfolgreicher Anmeldung
            this.router.navigate(['/home']);
            this.errorResponse = false;
          } else {
            // Fehlerbehandlung für den Fall, dass kein Token zurückgegeben wurde
            console.error('Token not found in response');
          }
        },
        (error) => {
          console.error(error);
          this.errorResponse = true;
          setTimeout(() => {
            this.errorResponse = false;
          }, 5000);
        }
      );
    } else {
      console.log('Invalid Data');
      this.errorResponse = true;
      setTimeout(() => {
        this.errorResponse = false;
      }, 5000);
    }

    this.password = '';
    this.email = '';
  }

  RegisterUser() {
    if (this.isValidEmail(this.registeremail)) {
      const body = {
        email: this.registeremail,
        password: this.registerpassword,
        name: this.name,
      };
      console.log(this.registeremail + ' ' + this.registerpassword);
      this.client.post(this.url + '/sign', body).subscribe(
        (response) => {
          console.log('Response:', response);
          // Weiterleitung oder andere Aktionen nach erfolgreicher Anmeldung
          this.router.navigate(['/home']);
          this.errorResponse = false;
        },
        (error) => {
          console.error('Error:', error);
          this.errorResponse = true;
          setTimeout(() => {
            this.errorResponse = false;
          }, 5000);
        }
      );
    } else {
      console.log('Invalid Data');
      console.log(this.registeremail + ' ' + this.registerpassword + 'Penis');
      this.errorResponse = true;
      setTimeout(() => {
        this.errorResponse = false;
      }, 5000);
    }

    this.registeremail = '';
    this.registerpassword = '';
    this.name = '';
  }

  GetUserInformations() {
    const jwtToken = localStorage.getItem('token');
    console.log(jwtToken);
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: `Bearer ${jwtToken}`,
      }),
    };
    this.client.get(this.url + '/Name', httpOptions).subscribe(
      (data) => {
        // Hier kannst du mit den empfangenen Daten arbeiten
        console.log('Daten empfangen:', data);
        this.datas = data; // Speichere die empfangenen Daten in this.datas oder führe andere Operationen damit aus
      },
      (error) => {
        // Hier kannst du Fehlerbehandlung durchführen
        console.error('Fehler beim Abrufen der Daten:', error);
      }
    );
    console.log(this.datas);
  }
}
