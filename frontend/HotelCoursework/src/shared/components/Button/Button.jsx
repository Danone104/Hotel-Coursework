import { memo } from 'react'
import styles from './Button.module.css'
const Button = (props)=>{
    const{
        children,
        isDisabled,
        onClick,
        type= 'button',
        className=''
    }=props
    return(
        <button 
        type={type} 
        onClick={onClick} 
        disabled={isDisabled} 
        className={`${styles.button} ${className}`}
        >
            {children}
        </button>
    )
}
export default memo(Button)