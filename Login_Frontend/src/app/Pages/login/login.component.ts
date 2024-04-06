import { Component } from '@angular/core';
import { AuthentifikationService } from '../../services/authentifikation.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  passwordFieldType: string = "password"; 
  img: string = "../../../assets/ausblenden.png"; 
  showPassword: boolean = false;
constructor(public auth: AuthentifikationService)
{
  this.auth = auth; 
}

toggleType()
{
 
  this.showPassword = !this.showPassword;
  this.passwordFieldType = this.showPassword ? 'text' : 'password';
  this.img = this.showPassword ? '../../../assets/ausblenden.png' : '../../../assets/aussicht.png' 

}


}
