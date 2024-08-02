import{dxFlyoutTagName as t}from"./flyout-025ae9cf.js";import{D as e}from"./popupportal-baf3cf8a.js";import{d as o}from"./events-interseptor-e0b24870.js";import{a as r,b as s,x as i,y as p}from"./popup-4f623924.js";import{_ as m}from"./_tslib-6e8ca86b.js";import{e as a}from"./property-d3853089.js";import{e as n}from"./custom-element-267f9a21.js";import{s as c,i as d,y as f}from"./lit-element-70cf14f4.js";import"./point-e4ec110e.js";import"./layouthelper-1d804c8c.js";import"./constants-58283e53.js";import"./query-44b9267f.js";import"./rafaction-bba7928b.js";import"./transformhelper-ebad0156.js";import"./positiontracker-d7595cd2.js";import"./branch-bf06b0d2.js";import"./capturemanager-89a507c8.js";import"./eventhelper-8570b930.js";import"./dispatcheraction-19309c7b.js";import"./logicaltreehelper-99f1adf9.js";import"./portal-9686dca9.js";import"./data-qa-utils-8be7c726.js";import"./constants-058ebb50.js";import"./dx-html-element-pointer-events-helper-1bf230d1.js";import"./dom-e3fa5208.js";import"./browser-f8f6e902.js";import"./common-eb095e4d.js";import"./devices-9f03a976.js";import"./dx-ui-element-db9e89a3.js";import"./lit-element-base-7a85dca5.js";import"./nameof-factory-64d95f5b.js";import"./custom-events-helper-e7f279d3.js";import"./key-fa0d8a82.js";import"./dx-keyboard-navigator-159220aa.js";import"./focus-utils-6f61e33c.js";import"./touch-167bb2e4.js";import"./disposable-d2c2d283.js";import"./dom-utils-751497ba.js";import"./css-classes-f45f6949.js";const l="dxbl-flyout-root";let j=class extends c{constructor(){super(...arguments),this.dropOpposite=!1,this.dropDirection=r.Near,this.dropAlignment=s.bottom}static get styles(){return d`
            :host {
                display: flex;
                flex: 1 1 auto;
                flex-direction: column;
                align-items: stretch;
                justify-items: stretch;
                min-height: 0;
            }
        }`}render(){return f`
            <slot></slot>
            <slot name="arrow"/>
        `}};m([a({type:Object,attribute:"drop-opposite"})],j.prototype,"dropOpposite",void 0),m([a({type:String,attribute:"drop-direction"})],j.prototype,"dropDirection",void 0),m([a({type:String,attribute:"drop-alignment"})],j.prototype,"dropAlignment",void 0),j=m([n(l)],j);function b(t){if(!t)throw new Error("failed");return t}const u={getReference:b,registeredComponents:[t,i,l,e,o,p]};export{u as default,b as getReference};
