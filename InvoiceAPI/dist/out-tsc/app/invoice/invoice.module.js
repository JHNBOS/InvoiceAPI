"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var common_1 = require("@angular/common");
var http_1 = require("@angular/common/http");
var forms_1 = require("@angular/forms");
var invoice_service_1 = require("../shared/services/invoice.service");
var debtor_service_1 = require("../shared/services/debtor.service");
var address_service_1 = require("../shared/services/address.service");
var lives_at_service_1 = require("../shared/services/lives-at.service");
var located_at_service_1 = require("../shared/services/located-at.service");
var invoice_routing_1 = require("./invoice.routing");
var ng2_toasty_1 = require("ng2-toasty");
var error_handler_1 = require("../shared/error-handler");
var invoice_component_1 = require("./invoice.component");
var create_invoice_component_1 = require("./create-invoice/create-invoice.component");
var debtor_dropdown_module_1 = require("../debtor/debtor-dropdown/debtor-dropdown.module");
var InvoiceModule = /** @class */ (function () {
    function InvoiceModule() {
    }
    InvoiceModule = __decorate([
        core_1.NgModule({
            declarations: [
                invoice_component_1.InvoiceComponent,
                create_invoice_component_1.CreateInvoiceComponent
            ],
            imports: [
                common_1.CommonModule,
                http_1.HttpClientModule,
                forms_1.FormsModule,
                debtor_dropdown_module_1.DebtorDropdownModule,
                ng2_toasty_1.ToastyModule.forRoot(),
                invoice_routing_1.InvoiceRoutingModule
            ],
            providers: [
                invoice_service_1.InvoiceService,
                debtor_service_1.DebtorService,
                address_service_1.AddressService,
                lives_at_service_1.LivesAtService,
                located_at_service_1.LocatedAtService,
                { provide: core_1.ErrorHandler, useClass: error_handler_1.CustomErrorHandler }
            ],
            exports: [
                invoice_component_1.InvoiceComponent
            ]
        })
    ], InvoiceModule);
    return InvoiceModule;
}());
exports.InvoiceModule = InvoiceModule;
//# sourceMappingURL=invoice.module.js.map