import { Component } from '@angular/core';
import { AuthentifikationService } from '../../services/authentifikation.service';

@Component({
  selector: 'app-sign',
  templateUrl: './sign.component.html',
  styleUrl: './sign.component.scss'
})
export class SignComponent {
  passwordlenght: boolean = false; 
  passwordsymbols: boolean = false; 
  passwordnumbers: boolean = false; 
  buttondisabled: boolean = true; 



  
constructor(public auth: AuthentifikationService)
{
  this.auth = auth; 
}

checkpassword()
{
  var password = this.auth.registerpassword; 
  if(password.length >= 8)
  {
    this.passwordlenght = true;
  }else
  {
    this.passwordlenght = false;
  }

  if(this.containsSpecialSymbols(password))
  {
    this.passwordsymbols = true;
  }else{
    this.passwordsymbols = false; 
  }

  if(this.containsNumbersInString(password))
  {
    this.passwordnumbers = true; 
  }else
  {
    this.passwordnumbers = false; 
  }

  if(this.passwordlenght && this.passwordnumbers && this.passwordsymbols)
  {
      this.buttondisabled= false;
  }else
  {
    this.buttondisabled = true; 
  }
  console.log(password); 
}

containsSpecialSymbols(password: string)
{
  var specialSymbols = /[.!@#$%^*\-_+=]/;
  return specialSymbols.test(password);
}

containsNumbersInString(password: string)
{
  var specialSymbols = /\d/; 
  return specialSymbols.test(password); 
}
}
