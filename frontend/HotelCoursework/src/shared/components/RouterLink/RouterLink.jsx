const RouterLink = (props)=>{
    const {
        to,
        children,
        className='',
    }=props
    return(
        <a href={to} className={className}>
            {children}
        </a>
    )
}
export default RouterLink