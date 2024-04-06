import { Component, OnInit } from '@angular/core';
import { AuthentifikationService } from '../../services/authentifikation.service';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {


constructor(public auth: AuthentifikationService)
{
  this.auth = auth; 
}
  ngOnInit(): void {
    this.auth.GetUserInformations(); 
  }



}
